using System;
using System.Runtime.InteropServices;

namespace RoboComp
{
	internal class Motor
	{
		#region Direction Enum

		public enum Direction { None, Forward, Reverse }

		#endregion

		#region Extern Methods

		[DllImport("mtb.dll")]
		private static extern bool InitMotoBee();

		[DllImport("mtb.dll")]
		private static extern bool Digital_IO(ref int inputs, int outputs);

		[DllImport("mtb.dll")]
		private static extern bool SetMotors(int on1, int speed1, int on2, int speed2, int on3, int speed3, int on4, int speed4, int servo);

		#endregion

		#region Private Members

		private readonly int cruiseSpeed, speedDelta, turnStep;

		#endregion

		#region Constructors

		public Motor(int cruiseSpeed, int speedDelta, int turnStep)
		{
			this.cruiseSpeed = cruiseSpeed;
			this.speedDelta = speedDelta;
			this.turnStep = turnStep;
		}

		#endregion

		#region Public Members

		public bool Started { get; private set; }

		public int LeftSpeed { get; private set; }

		public int RightSpeed { get; private set; }

		public Direction CurrentDirection { get; private set; }

		#endregion

		#region Public Methods

		public bool Start()
		{
			Started = InitMotoBee();

			return Started;
		}
	
		public bool Forward()
		{
			ValidateState();

			bool success = SetMotors(1, LeftSpeed + speedDelta, 0, 0, 1, RightSpeed, 0, 0, 0);

			if (success)
			{
				CurrentDirection = Direction.Forward;
			}

			return success;
		}

		public bool Reverse()
		{
			ValidateState();

			bool success = SetMotors(0, 0, 1, LeftSpeed + speedDelta, 0, 0, 1, RightSpeed, 0);

			if (success)
			{
				CurrentDirection = Direction.Reverse;
			}

			return success;
		}

		public bool Stop()
		{
			bool success = SetMotors(0, 0, 0, 0, 0, 0, 0, 0, 0);

			if (success)
			{
				LeftSpeed = 0;
				RightSpeed = 0;
				CurrentDirection = Direction.None;
			}

			return success;
		}

		public void TurnLeft()
		{
			LeftSpeed = 0;

			ContinueMoving();
		}

		public void TurnRight()
		{
			RightSpeed = 0;

			ContinueMoving();
		}

		public void StopTurning()
		{
			LeftSpeed = cruiseSpeed;
			RightSpeed = cruiseSpeed;

			ContinueMoving();
		}

		#endregion

		#region Private Methods

		private void ValidateState()
		{
			if (!Started)
			{
				Start();
			}

			if (CurrentDirection == Direction.None)
			{
				LeftSpeed = cruiseSpeed;
				RightSpeed = cruiseSpeed;
			}
		}

		private void ContinueMoving()
		{
			if (CurrentDirection == Direction.Forward)
			{
				Forward();
			}
			else if (CurrentDirection == Direction.Reverse)
			{
				Reverse();
			}
		}

		#endregion
	}
}
