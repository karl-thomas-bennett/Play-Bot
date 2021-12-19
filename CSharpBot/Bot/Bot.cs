using System;
using System.Drawing;
using System.Numerics;
using Bot.Utilities.Processed.BallPrediction;
using Bot.Utilities.Processed.FieldInfo;
using Bot.Utilities.Processed.Packet;
using RLBotDotNet;
using Bot.scripts;

namespace Bot
{
    // We want to our bot to derive from Bot, and then implement its abstract methods.
    class Bot : RLBotDotNet.Bot
    {
        PID steeringController;
        string team;
        // We want the constructor for our Bot to extend from RLBotDotNet.Bot, but we don't want to add anything to it.
        // You might want to add logging initialisation or other types of setup up here before the bot starts.
        public Bot(string botName, int botTeam, int botIndex) : base(botName, botTeam, botIndex) {
            
            steeringController = new PID(-1, 1, (measurement, prevMeasurement, goal) => {
                float PI = (float)Math.PI;
                float rotationsToGoal = (float)Math.Floor((goal - prevMeasurement) / (2 * PI));
                float rotations = (float)Math.Floor(prevMeasurement / (2 * PI));
                return new float[] { measurement + 2 * PI * rotations, goal + 2 * PI * rotationsToGoal };
            });
            team = botTeam == 0 ? "Blue" : "Orange";
        }

        public override Controller GetOutput(rlbot.flat.GameTickPacket gameTickPacket)
        {
            // We process the gameTickPacket and convert it to our own internal data structure.
            Packet packet = new Packet(gameTickPacket);

            // Get the data required to drive to the ball.
            Vector3 ballLocation = packet.Ball.Physics.Location;
            Vector3 carLocation = packet.Players[Index].Physics.Location;
            Orientation carRotation = packet.Players[Index].Physics.Rotation;

            // Find where the ball is relative to us.
            Vector3 ballRelativeLocation = Orientation.RelativeLocation(carLocation, ballLocation, carRotation);

            // Decide which way to steer in order to get to the ball.
            // If the ball is to our left, we steer left. Otherwise we steer right.
            float steer;
            if (ballRelativeLocation.Y > 0)
                steer = 1;
            else
                steer = -1;
            
            // Examples of rendering in the game

            BallPrediction prediction = GetBallPrediction();
            PredictionSlice slice = new PredictionSlice();
            //Console.WriteLine(prediction.Slices.Length);
            for(int i = 0; i < prediction.Slices.Length - 1; i++)
            {
                if(prediction.Slices[i].Physics.Location.Z > 95 && prediction.Slices[i + 1].Physics.Location.Z < 95)
                {
                    slice = prediction.Slices[i + 1];
                    break;
                }
                if(i == prediction.Slices.Length - 2)
                {
                    slice = prediction.Slices[i];
                }
            }
            if (!packet.GameInfo.IsKickoffPause && prediction.Slices.Length > 1)
            {
                Vector3 location = slice.Physics.Location;
                scripts.Vector goal = new scripts.Vector(location).Flatten();
                scripts.Vector forward = new scripts.Vector(carRotation.Forward).Flatten();
                Console.WriteLine(forward.SignedAngleTo(goal));
                steer = steeringController.P(1).I(1).D(1).SetGoal(goal.AngleOfDirection()).Next(forward.SignedAngleTo(goal));
                Renderer.DrawLine3D(Color.Red, carLocation, goal.ToVector3());
            }
            
            // This controller will contain all the inputs that we want the bot to perform.
            return new Controller
            {
                // Set the throttle to 1 so the bot can move.
                Throttle = 1,
                Steer = steer
            };
        }
        
        // Hide the old methods that return Flatbuffers objects and use our own methods that
        // use processed versions of those objects instead.
        internal new FieldInfo GetFieldInfo() => new FieldInfo(base.GetFieldInfo());
        internal new BallPrediction GetBallPrediction() => new BallPrediction(base.GetBallPrediction());
    }
}