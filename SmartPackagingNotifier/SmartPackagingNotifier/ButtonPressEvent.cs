using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartPackagingNotifier
{
    public class ButtonPressEvent
    {
        public int Button { get; set; }
        public string DeviceType { get; set; } //LabelScanner, Doorbell, Signaling
        public string DeviceId { get; set; }

        public string Metadata { get; set; }
    }

    public class CommandEvent<T> where T : IPayload
    {
        public DateTime TimeStamp { get; set; }
        public string Type { get { return Payload.Type; } }
        public T Payload { get; set; }
    }
    public interface IPayload
    {
        string Type { get; }
    }

    public class PackageArrivedEvent : IPayload
    {
        public string Location { get; set; }
        public string Type { get; } = "PackageArrivedEvent";
    }

    public class PackagePickerConfirmedEvent : IPayload
    {
        public string DeviceId { get; set; }
        public string Type { get; } = "PackagePickerConfirmedEvent";
        public string Location { get; set; }
    }

    public class PackageLabelScanEvent : IPayload
    {
        public string DeviceId { get; set; }
        public string Path { get; set; }
        public string Type { get; } = "PackageLabelScanEvent";

    }

}
