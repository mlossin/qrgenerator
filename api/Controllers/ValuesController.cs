using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QRCoder.Exceptions;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net;

namespace QRCodeGenerator.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public IActionResult Get()
        {
            return File(CreateQrCode("please insert a text"), "image/jpeg"); 
        }

        private Byte[] CreateQrCode(string v)
        {
            QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(v, QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            //convert Bitmap to ByteArray in MemoryStream (https://stackoverflow.com/questions/7350679/convert-a-bitmap-into-a-byte-array)
            using (var stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                Byte[] byteBild = stream.ToArray();
                return byteBild;
            }
            //ImageConverter converter = new ImageConverter(); //https://stackoverflow.com/questions/7350679/convert-a-bitmap-into-a-byte-array
        }
        private Byte[] CreateRefraQrCode(string v)
        {
            QRCoder.QRCodeGenerator qrGenerator = new QRCoder.QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(v, QRCoder.QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            Bitmap refraIcon = GetRefraLogo().Result; //(Bitmap)Bitmap.FromFile("");            // https://www.refra.com/img/icon/favicon-196x196.png
            //Bitmap qrCodeImage = qrCode.GetGraphic(20, "#ff4c02", "#000000", true);
            //Color darColor = System.Drawing.ColorTranslator.FromHtml("#ff4c02");
            Color darColor = System.Drawing.ColorTranslator.FromHtml("#000000");
            Color lightColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");
            Bitmap qrCodeImage = qrCode.GetGraphic(20,darColor ,lightColor , refraIcon,10,5,true);
            using (var stream = new MemoryStream())
            {
                qrCodeImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                Byte[] byteBild = stream.ToArray();
                return byteBild;
            }
        }

        private async Task<Bitmap> GetRefraLogo()
        {
            var images = new List<Bitmap>();
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://www.refra.com/img/icon/favicon-196x196.png"); // https://www.refra.com/img/icon/favicon-196x196.png
                //var bitmap = new Bitmap();
                if (response != null && response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        var memStream = new MemoryStream();
                        await stream.CopyToAsync(memStream);
                        memStream.Position = 0;
                        images.Add(new Bitmap(memStream));
                    }
                }
            }
            return images.FirstOrDefault();
        }


        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(string id)
        {
            return File(CreateRefraQrCode(id), "image/jpeg");
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
