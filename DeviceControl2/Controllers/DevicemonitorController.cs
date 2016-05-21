using Microsoft.Azure.Devices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DeviceControl2.Controllers
{
    public class DevicemonitorController : Controller
    {
        static RegistryManager registryManager;
        
        public ActionResult Index()
        {
            if (registryManager == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["iothubConnectionString"].ToString();
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            }
            IEnumerable<Device> deviceList = null;
            var runSync = Task.Factory.StartNew(new Func<Task>(async () =>
            {
                deviceList = await registryManager.GetDevicesAsync(10);
            })).Unwrap();
            runSync.Wait();
            //int num = 1;
            //deviceList.ToArray();
            ViewData["length"] = deviceList.Count();
            int i = 0;
            foreach (var deviceSplit in deviceList)
            {
                ViewData["DeviceId"+ i.ToString()] = deviceSplit.Id;
                ViewData["DeviceKey" + i.ToString()] = deviceSplit.Authentication.SymmetricKey.PrimaryKey;
                //ViewData["ConnectionStatus" + i.ToString()] = deviceSplit.ConnectionState;
                ViewData["DeviceHistory" + i.ToString()] = deviceSplit.LastActivityTime.ToString();
                ViewData["DeviceStatus" + i.ToString()] = deviceSplit.Status;
                ViewData["Platform" + i.ToString()] = deviceSplit.Id;
                ViewData["C2DCount" + i.ToString()] = deviceSplit.CloudToDeviceMessageCount.ToString();
                ViewData["PushButton" + i.ToString()] = "Push" + i.ToString();
                i++;
            }

           
            return View();
        }
        static ServiceClient serviceClient;
        public async Task<ActionResult> PushButton(string deviceKey)
        {

            string connectionString = ConfigurationManager.ConnectionStrings["iothubConnectionString"].ToString(); ;
            serviceClient = ServiceClient.CreateFromConnectionString(connectionString);
            await SendCloudToDeviceMessageAsync(deviceKey);
            //await ReceiveFeedbackAsync();
            return Redirect("/Devicemonitor/Index");

        }
        private async Task SendCloudToDeviceMessageAsync(string deviceId)
        {
            var commandMessage = new Message(Encoding.ASCII.GetBytes("{\"data\": \"1\"}"));
            commandMessage.Ack = DeliveryAcknowledgement.Full;
            await serviceClient.SendAsync(deviceId, commandMessage);
        }
        //private async Task ReceiveFeedbackAsync()
        //{
        //    var feedbackReceiver = serviceClient.GetFeedbackReceiver();

        //    while (true)
        //    {
        //        var feedbackBatch = await feedbackReceiver.ReceiveAsync();
        //        if (feedbackBatch == null) continue;
        //        if (feedbackBatch.Records.ToArray()[0].StatusCode == FeedbackStatusCode.Success)
        //        Response.Write("<script>window.alert('success');</script>");
        //        //Console.WriteLine("Received feedback: {0}", string.Join(", ", feedbackBatch.Records.Select(f => f.StatusCode)));
        //        await feedbackReceiver.CompleteAsync(feedbackBatch);
        //    }
        //}

    }
}