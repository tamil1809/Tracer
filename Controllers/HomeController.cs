﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tracer.Attributes;
using System.Net.WebSockets;
using Tracer.Models;

namespace Tracer.Controllers
{
    public class HomeController : Controller
    {
        private IPusher<WebSocket> _pusher;
        private static byte[] _data;
        private static int length = 1024 * 1024 * 3;
        private static byte[] GetData()
        {
            if (_data == null)
            {
                _data = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    _data[i] = 1;
                }
            }
            return _data;
        }

        private static Object message = new { message = "ok" };
        public HomeController()
        {
            _pusher = new WebSocketPusher();
        }

        public IActionResult Index()
        {
            return View();
        }

        [ForceWS]
        public async Task<IActionResult> Pushing()
        {
            await _pusher.Accept(HttpContext);
            while (_pusher.Connected)
            {
                try
                {
                    await _pusher.SendMessage(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffffff"));
                    await Task.Delay(10);
                }
                catch
                {
                    break;
                }
            }
            return null;
        }

        public IActionResult Ping()
        {
            return Json(message);
        }

        public IActionResult Download()
        {
            HttpContext.Response.Headers.Add("Content-Length", length.ToString());
            HttpContext.Response.Headers.Add("cache-control", "no-cache");
            return new FileContentResult(GetData(), "application/octet-stream");
        }

        public IActionResult Address()
        {
            return Json(new
            {
                remoteIpAddress = HttpContext.Connection.RemoteIpAddress.ToString()
            });
        }
    }
}
