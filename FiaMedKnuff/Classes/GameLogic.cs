namespace FiaMedKnuff.Classes
{
    public class GameLogic
    {
        public bool VictoryCheck(GamePiece[] pieces)
        {
            int gamePiecesInSafeZone = 0;
            foreach (GamePiece piece in pieces)
            {
                if (piece.StepsTaken == 45)
                    gamePiecesInSafeZone++;
            }

            return gamePiecesInSafeZone == 4;
        }
    }
}