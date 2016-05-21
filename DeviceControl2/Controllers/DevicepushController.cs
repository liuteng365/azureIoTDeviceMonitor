using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DeviceControl2.Controllers
{
    //no use
    public class DevicepushController : Controller
    {
        static ServiceClient serviceClient;
        public ActionResult PushButton(string deviceKey)
        {

            string connectionString = ConfigurationManager.ConnectionStrings["iothubConnectionString"].ToString(); ;
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            SendCloudToDeviceMessageAsync(deviceKey);
            var script = string.Format("Success");
            return JavaScript(script);

        }
        private async Task SendCloudToDeviceMessageAsync(string deviceKey)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message."));
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            await serviceClient.SendAsync(deviceKey, commandMessage);
        }

        //private async Task< void ReceiveFeedbackAsync()
        //{
        //    var feedbackReceiver = serviceClient.GetFeedbackReceiver();
        //    while (true)
        //    {
        //        var feedbackBatch = await feedbackReceiver.ReceiveAsync();
        //        if (feedbackBatch == null) continue;              
        //        await feedbackReceiver.CompleteAsync(feedbackBatch);
        //        return string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode))
        //    }
        //}
    }
}
