using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using IotBackend.Api.Infrastructure.Exceptions;
using IotBackend.Api.Infrastructure.Handlers;
using IotBackend.Api.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;

namespace IotBackend.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1")]
    public class DevicesController : ControllerBase
    {
        private readonly IDevicesHandler _handler;

        public DevicesController(IDevicesHandler handler)
        {
            _handler = handler;
        }

        [HttpGet]
        [Route("data/{deviceName}/{sensorType}/{date}")]
        public async Task<ActionResult<List<ISensorData>>> GetDeviceSensorDailyData(string deviceName, string sensorType, DateTime date)
        {
            try
            {
                var result = await _handler.HandleGetDeviceSensorDailyData(deviceName,sensorType, date);
                return Ok(result);
            }
            catch (BaseException baseException)
            {
                return new ContentResult()
                {
                    StatusCode = baseException.StatusCode,
                    Content = baseException.Message,
                };
            }
            catch (Exception)
            {
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Content = "Uexcpected server error occured",
                };
            }
        }

        [HttpGet]
        [Route("data/{devicename}/{date}")]
        public async Task<ActionResult<List<DeviceData>>> GetDeviceDailyData(string deviceName, DateTime date)
        {
            try
            {
                var result = await _handler.HandleGetDeviceDailyData(deviceName, date);
                return Ok(result);
            }
            catch (BaseException baseException)
            {
                return new ContentResult()
                {
                    StatusCode = baseException.StatusCode,
                    Content = baseException.Message,
                };
            }
            catch (Exception)
            {return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Content = "Uexcpected server error occured",
                };
            }
        }
    }
}