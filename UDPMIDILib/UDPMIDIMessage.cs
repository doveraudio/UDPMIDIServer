using Newtonsoft.Json;
namespace UDPMIDILib
{
    public class UDPMIDIMessage
    {
        public UDPMIDIMessage(string mode, string value)
        {
            this.Value = value;
            this.Mode = mode;
        }
        private string mode;
        private string value;

        public string Mode { get => mode; set => mode = value; }
        public string Value { get => value; set => this.value = value; }

        public string ToJson()
        {

            return JsonConvert.SerializeObject(this);
        }
    }
}
