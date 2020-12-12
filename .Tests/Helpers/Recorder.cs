namespace Hopper.Tests
{
    public class Recorder
    {
        public string recordedSequence = "";
        public void Record(string message)
        {
            recordedSequence += message;
        }
    }
}