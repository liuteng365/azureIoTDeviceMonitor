using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace DeviceControl2.Controllers
{
    public class DeviceInfoController : ApiController
    {
        static RegistryManager registryManager;
        [HttpGet]
        public async Task<string> GetDeviceInfo()
        {
            if (registryManager == null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["iothubConnectionString"].ToString();
                registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            }
            IEnumerable<Device> deviceList = await registryManager.GetDevicesAsync(10);
            return "1";
        }
        [HttpPost]
        public async Task<string> DeviceRegister(string deviceName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["iothubConnectionString"].ToString();
            registryManager = RegistryManager.CreateFromConnectionString(connectionString);
            string deviceKey = await AddDeviceAsync(deviceName);
            return deviceKey;
        }
        private async Task<string> AddDeviceAsync(string deviceId)
        {
            Device device = null;
            bool deviceexsits = false;
            try
            {
                device = await registryManager.AddDeviceAsync(new Device(deviceId));
                deviceexsits = true;
            }
            catch (DeviceAlreadyExistsException)
            {
                deviceexsits = false;
            }

            if (deviceexsits == false)
            {
                device = await registryManager.GetDeviceAsync(deviceId);
            }
            return device.Authentication.SymmetricKey.PrimaryKey;
        }
    }
}
