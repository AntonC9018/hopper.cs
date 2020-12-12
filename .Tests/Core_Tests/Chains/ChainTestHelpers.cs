using Hopper.Utils.Chains;

namespace Hopper.Tests
{
    public class ControlledHandler
    {
        public string message;
        public Recorder recorder;

        public ControlledHandler(string message, Recorder recorder)
        {
            this.message = message;
            this.recorder = recorder;
        }

        public void Function(EventBase ev)
        {
            recorder.Record(message);
        }
    }
}