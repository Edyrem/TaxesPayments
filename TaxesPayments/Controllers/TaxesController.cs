using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using TaxesPayments.Data;
using TaxesPayments.Helpers;
using TaxesPayments.Migrations;
using TaxesPayments.Models;

namespace TaxesPayments.Controllers
{
    public class TaxesController : Controller
    {
        private readonly TaxesPaymentsContext _context;

        private readonly string url = "https://api.quickpay.kg:9202?";

        private static readonly HttpClient client = new HttpClient();

        private static Taxes globalTaxes = new Taxes();

        private static Dictionary<string, string> _baipOptions = new Dictionary<string, string>();

        private int point = 12439003;
        private string skey = "f3r86ipt5q";
        private int service = 3885;
        private string certificateName = $"wwwroot/files/12439003.p12";
        private string certificatePassword = "386LbpITo#4c";
        // combo2 is Taxes payment code - Налог на транспортные средства физических лиц
        private string taxServiceNumber = "3885";

        private string productCode = "2280007001509";
        private string partnerCode = "2280007001509";


        public TaxesController(TaxesPaymentsContext context)
        {
            _context = context;
        }

        // GET: Taxes
        public async Task<IActionResult> Index()
        {
            //return _context.Taxes != null ?
            //            View(await _context.Taxes.ToListAsync()) :
            //            Problem("Entity set 'TaxesPaymentsContext.Taxes'  is null.");
            return View();
        }

        public IActionResult Check(string bc, string bc_CID, string bc_SUM, string cashregister_id, string kiosk_id, string receipt_id, string productcode, string language, string partner_id, string user_name, string time)
        {
            HttpContext.Response.Cookies.Append("bc", "" + bc);
            HttpContext.Response.Cookies.Append("bc_CID", "" + bc_CID);
            HttpContext.Response.Cookies.Append("bc_SUM", "" + bc_SUM);

            HttpContext.Response.Cookies.Append("cashregister_id", "" + cashregister_id);
            HttpContext.Response.Cookies.Append("kiosk_id", "" + kiosk_id);
            HttpContext.Response.Cookies.Append("receipt_id", "" + receipt_id);
            HttpContext.Response.Cookies.Append("productcode", "" + productcode);
            HttpContext.Response.Cookies.Append("language", "" + language);
            HttpContext.Response.Cookies.Append("partner_id", "" + partner_id);
            HttpContext.Response.Cookies.Append("user_name", "" + user_name);
            HttpContext.Response.Cookies.Append("time", "" + time);

            string host_ip = HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            if (_baipOptions.ContainsKey("cashregister_id")) { _baipOptions["cashregister_id"] = cashregister_id; } else { _baipOptions.Add("cashregister_id", cashregister_id); }
            if (_baipOptions.ContainsKey("kiosk_id")) { _baipOptions["kiosk_id"] = kiosk_id; } else { _baipOptions.Add("kiosk_id", kiosk_id); }
            if (_baipOptions.ContainsKey("receipt_id")) { _baipOptions["receipt_id"] = receipt_id; } else { _baipOptions.Add("receipt_id", receipt_id); }
            if (_baipOptions.ContainsKey("productcode")) { _baipOptions["productcode"] = productcode; } else { _baipOptions.Add("productcode", productcode); }
            if (_baipOptions.ContainsKey("language")) { _baipOptions["language"] = language; } else { _baipOptions.Add("language", language); }
            if (_baipOptions.ContainsKey("partner_id")) { _baipOptions["partner_id"] = partner_id; } else { _baipOptions.Add("partner_id", partner_id); }
            if (_baipOptions.ContainsKey("user_name")) { _baipOptions["user_name"] = user_name; } else { _baipOptions.Add("user_name", user_name); }
            if (_baipOptions.ContainsKey("time")) { _baipOptions["time"] = time; } else { _baipOptions.Add("time", time); }
            if (_baipOptions.ContainsKey("host_ip")) { _baipOptions["host_ip"] = host_ip; } else { _baipOptions.Add("host_ip", host_ip); }

            return View();
        }

        // POST: Payments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Check([Bind("persacc, comment, sum")] Taxes taxes)
        {
            try
            {
                //string url = "https://api.quickpay.kg:9202?act=prepay&srv=3879&xml=";
                taxes.time = DateTime.Now;
                taxes.point = point;
                taxes.skey = skey;
                taxes.service = service;
                taxes.quickPayId = GenerateQuickPayId();

                var certificate = new X509Certificate2(certificateName, certificatePassword);
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(certificate);

                //string xmlResponce = @$"<root>
                //                     <header>
                //                      <time>{taxes.time}</time>
                //                      <point>{taxes.point}</point>
                //                      <skey>{taxes.skey}</skey>
                //                     </header>
                //                     <operation>
                //                      <service>{taxes.service}</service>
                //                      <account>
                //                       <value>
                //                        <persacc>{taxes.persacc}</persacc>
                //                       </value>
                //                      </account>
                //                     </operation>
                //                    </root>";
                string xmlResponce = @$"<root><header><time>{taxes.time}</time><point>{taxes.point}</point><skey>{taxes.skey}</skey></header><operation id=""{taxes.quickPayId}""><service>{taxes.service}</service><account><value><persacc>{taxes.persacc}</persacc></value></account></operation></root>";


                var fullUrl = url + $"act=prepay&srv={taxServiceNumber}&xml=" + xmlResponce;
                var apiResponse = "";
                var xroot = new XDocument();
                using (var httpClient = new HttpClient(handler))
                {
                    using(var response = await httpClient.GetAsync(fullUrl))
                    {
                        apiResponse += await response.Content.ReadAsStringAsync();
                        var stream = await response.Content.ReadAsStreamAsync();
                        var streamReader = new StreamReader(stream, Encoding.UTF8);
                        apiResponse = streamReader.ReadToEnd();
                        xroot = XDocument.Parse(apiResponse);
                        stream.Close();
                        streamReader.Close();
                    }
                }

                var parsedXml = XElement.Parse(apiResponse);
                var doc = new XmlDocument();
                doc.LoadXml(apiResponse);

                var dictionary = new Dictionary<string, Dictionary<string, string>>();
                
                foreach(XElement element in xroot.Root.Elements("operation"))
                {
                    AddToDictionary(element, dictionary);
                }

                if (dictionary.ContainsKey("err") && dictionary["err"]["err"] == "0")
                {
                    taxes.accountName = dictionary["fio"]["value"];
                    globalTaxes = taxes;
                    //return View("Pay", taxes);
                    return RedirectToAction("Pay");
                }
                //var msg = dictionary["msg"]["msg"];
                ViewBag.Error = dictionary["msg"]["msg"];
                return View("Check");
            }
            //catch (HttpRequestException e)
            catch (Exception e)
            {
                ViewBag.Error = "Запрос не отработан по причине: " + e.Message + " " + e.InnerException?.Message;
                //Console.WriteLine("\nException Caught!");
                //Console.WriteLine("Message :{0} ", e.Message);
            }
            return View();
        }

        public async Task<IActionResult> Pay(Taxes taxes, string abs = "abs")
        {
            try
            {
                taxes = globalTaxes;

                var states = XDocument.Load("wwwroot/files/States.xml");
                var districts = XDocument.Load("wwwroot/files/Districts.xml");

                var statesList = new Dictionary<string, string>();
                var districtsList = new Dictionary<string, Dictionary<string, string>>();

                foreach (XElement element in states.Elements())
                {
                    var list = new List<List<string>>();
                    GetDataFromExcelXml(element, list);
                    foreach (var l in list)
                    {
                        statesList.Add(l[0], l[1]);
                    }
                }

                foreach (XElement element in districts.Elements())
                {
                    AddToDictionary(element, districtsList, false);
                }

                ViewBag.States = statesList;
                ViewBag.Districts = districtsList["01"];

                return View("Pay", taxes);
            }
            catch (HttpRequestException e)
            {
                ViewBag.Error = "Запрос не отработан по причине: " + e.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayBAIP([Bind("carNumber")] Taxes taxes, string Combo1, string Combo3, string Combo4)
        {
            try
            {
                var certificate = new X509Certificate2(certificateName, certificatePassword);
                var handler = new HttpClientHandler();
                handler.ClientCertificates.Add(certificate);
                globalTaxes.carNumber = taxes.carNumber;
                taxes = globalTaxes;
                var combo4 = (Combo4 != null) ? $"<combo4>{Combo4}</combo4>" : "";
                string xmlResponce = $"<root>" +
                                        "<header>" +
                                            $"<time>{taxes.time}</time>" +
                                            $"<point>{taxes.point}</point>" +
                                            $"<skey>{taxes.skey}</skey>" +
                                        "</header>" +
                                        $"<operation id=\"{taxes.quickPayId}\">" +
                                            "<account>" +
                                                "<value>" +
                                                    $"<persacc>{taxes.persacc}</persacc>" +
                                                    $"<combo1>{Combo1}</combo1>" +
                                                    $"<combo2>{taxServiceNumber}</combo2>" +
                                                    $"<combo3>{Combo3}</combo3>" +
                                                    combo4 +
                                                    $"<ls>{taxes.carNumber}</ls>" +
                                                    $"<fio>{taxes.accountName}</fio>" +
                                                "</value>" +
                                            "</account>" +
                                            $"<real_pay>{taxes.sum}</real_pay>" +
                                            $"<total>{taxes.sum}</total>" +
                                            $"<service>{taxes.service}</service>" +
                                        "</operation>" +
                                    "</root>";

                //string xmlResponce = @$"<root><header><time>2022-12-27 10:36:26</time><point>12439003</point><skey>f3r86ipt5q</skey></header><operation id=""100066""><account><value><persacc>{taxes.persacc}</persacc><combo1>{Combo1}</combo1><combo2>{taxServiceName}</combo2><combo3>{Combo3}</combo3><combo4>{Combo3}</combo4><ls>{taxes.carNumber}</ls><fio>{taxes.accountName}</fio></value></account><real_pay>-10</real_pay><total>-10</total><service>{taxes.service}</service></operation></root>";

                var fullUrl = url + $"act=add&srv={taxServiceNumber}&xml=" + xmlResponce;
                var apiResponse = "";
                var xroot = new XDocument();
                using (var httpClient = new HttpClient(handler))
                {
                    using (var response = await httpClient.GetAsync(fullUrl))
                    {
                        apiResponse += await response.Content.ReadAsStringAsync();
                        var stream = await response.Content.ReadAsStreamAsync();
                        var streamReader = new StreamReader(stream, Encoding.UTF8);
                        apiResponse = streamReader.ReadToEnd();
                        try
                        {
                            xroot = XDocument.Parse(apiResponse);
                        }
                        catch (XmlException ex)
                        {
                            ViewBag.ErrorMessage = ex.Message;
                        }
                        stream.Close();
                        streamReader.Close();
                    }
                }

                var doc = new XmlDocument();
                doc.LoadXml(apiResponse);

                var dictionary = new Dictionary<string, Dictionary<string, string>>();

                foreach (XElement element in xroot.Root.Elements("operation"))
                {
                    AddToDictionary(element, dictionary);
                }

                if (dictionary.ContainsKey("err") && dictionary["err"]["err"] == "0")
                {
                    string baipUrl = "http://192.168.181.2/Operator.ashx";
                    //string url_ = "http://192.168.175.2/Operator.ashx";


                    _context.Taxes.Add(taxes);
                    _context.SaveChanges();
                    
                    var baip = new BAIP(client);
                    var soapXml = baip.SetSoapXml(taxes, _baipOptions, productCode, partnerCode, taxServiceNumber);
                    var content = baip.SetSoapRequest(baipUrl, soapXml);
                    try
                    {
                        XDocument responceBaip = new XDocument();
                        responceBaip = XDocument.Parse(content.Result.ToString());

                        var statusCode = responceBaip.Descendants().Where(x => x.Name.LocalName == "salesorder_response").Select(x =>
                            (string)x.Element(x.Name.Namespace + "status")
                        ).FirstOrDefault();
                        if (statusCode == "0")
                        {                            
                            _context.Taxes.Update(taxes);
                            await _context.SaveChangesAsync();
                            ViewBag.Message = "Запрос отработан успешно: средства внесены в счет";
                            return View();
                        }
                        else
                        {
                            var error = responceBaip.Descendants().Where(x => x.Name.LocalName == "salesorder_response").Select(x =>
                                (string)x.Element(x.Name.Namespace + "rejection_reason")
                             ).FirstOrDefault();
                            ViewBag.Message = "Запрос не отработан по причине: " + error;
                            return View();
                        }
                    }
                    catch(Exception ex)
                    {
                        ViewBag.Message = "Запрос не отработан по причине: " + ex.Message;
                    }

                    return View();
                }

                ViewBag.Error = dictionary["msg"]["msg"];
                return View(taxes);
            } 
            catch (HttpRequestException e)
            {
                ViewBag.Error = "Запрос не отработан по причине: " + e.Message;
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetLocations(string locationName, string locationCode)
        {
            try {
                var location = XDocument.Load($"wwwroot/files/{locationName}.xml");
                var locationList = new Dictionary<string, Dictionary<string, string>>();
                foreach (XElement element in location.Elements())
                {
                    AddToDictionary(element, locationList, false);
                }                
                var jsonResponse = locationList.ContainsKey(locationCode) ? JsonConvert.SerializeObject(locationList[locationCode]) : "{\"reponce\":\"false\"}";
                return Json(jsonResponse);
            }
            catch(Exception e)
            {
                var jsonResponse = JObject.Parse("{error:" + e.Message + "}");
                return Json(jsonResponse);
            }
            
        }
        #region hide

        // GET: Taxes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Taxes == null)
            {
                return NotFound();
            }

            var taxes = await _context.Taxes
                .FirstOrDefaultAsync(m => m.id == id);
            if (taxes == null)
            {
                return NotFound();
            }

            return View(taxes);
        }

        // GET: Taxes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Taxes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,time,point,skey,service,account,persacc,akey,err,msg,add")] Taxes taxes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taxes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taxes);
        }

        // GET: Taxes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Taxes == null)
            {
                return NotFound();
            }

            var taxes = await _context.Taxes.FindAsync(id);
            if (taxes == null)
            {
                return NotFound();
            }
            return View(taxes);
        }

        // POST: Taxes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,time,point,skey,service,account,persacc,akey,err,msg,add")] Taxes taxes)
        {
            if (id != taxes.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taxes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaxesExists(taxes.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(taxes);
        }

        // GET: Taxes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Taxes == null)
            {
                return NotFound();
            }

            var taxes = await _context.Taxes
                .FirstOrDefaultAsync(m => m.id == id);
            if (taxes == null)
            {
                return NotFound();
            }

            return View(taxes);
        }

        // POST: Taxes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Taxes == null)
            {
                return Problem("Entity set 'TaxesPaymentsContext.Taxes'  is null.");
            }
            var taxes = await _context.Taxes.FindAsync(id);
            if (taxes != null)
            {
                _context.Taxes.Remove(taxes);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaxesExists(int id)
        {
            return (_context.Taxes?.Any(e => e.id == id)).GetValueOrDefault();
        }

        #endregion

        public void AddToDictionary(XElement elements, Dictionary<string, Dictionary<string, string>> nameValue, bool AttributsRequired = true)
        {
            foreach(XElement element in elements.Elements())
            {
                if (AttributsRequired)
                {
                    var attrDictionary = new Dictionary<string, string>();
                    attrDictionary.Add(element.Name.ToString(), element.Value);                
                    foreach (XAttribute attribute in element.Attributes())
                    {
                        attrDictionary.Add(attribute.Name.ToString(), attribute.Value);
                    }
                    nameValue.Add(element.Name.ToString(), attrDictionary);
                    if (element.HasElements)
                    {
                        AddToDictionary(element, nameValue);
                    }
                }
                else
                {
                    var list = new List<List<string>>();
                    GetDataFromExcelXml(element, list);
                    foreach (var l in list)
                    {
                        if (nameValue.ContainsKey(l[0]))
                        {
                            nameValue[l[0]].Add(l[1], l[2]);
                        }
                        else
                        {
                            var d = new Dictionary<string, string>();
                            d.Add(l[1], l[2]);
                            nameValue.Add(l[0], d);
                        }
                    }
                }
            }
        }

        public void GetDataFromExcelXml(XElement document, List<List<string>> elementsList)
        {            
            foreach(XElement element in document.Elements())
            {
                if (element.Name.LocalName == "Row")
                {
                    var list = new List<string>();
                    foreach (XElement cell in element.Elements())
                        foreach (XElement data in cell.Elements())
                            list.Add(data.Value);
                    elementsList.Add(list);
                }
                else
                {
                    if (element.HasElements) GetDataFromExcelXml(element, elementsList);
                }
            }
            
        }

        public int GenerateQuickPayId()
        {
            var random = new Random();

            return random.Next(100000, 999999);
        }

    }
}
