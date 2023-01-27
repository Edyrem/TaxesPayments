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
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Common;
using TaxesPayments.Data;
using TaxesPayments.Migrations;
using TaxesPayments.Models;

namespace TaxesPayments.Controllers
{
    public class TaxesController : Controller
    {
        private readonly TaxesPaymentsContext _context;

        private readonly string url = "https://api.quickpay.kg:9202?act=prepay&srv=3879&xml=";

        private static readonly HttpClient client = new HttpClient();

        private static Taxes globalTaxes;

        private int point = 12439003;
        private string skey = "f3r86ipt5q";
        private int service = 3879;
        private string certificateName = "12439003.p12";
        private string certificatePassword = "386LbpITo#4c";


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
                TempData["time"] = taxes.time = DateTime.Now;
                TempData["point"] = taxes.point = point;
                TempData["skey"] = taxes.skey = skey;
                TempData["service"] = taxes.service = service;

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
                string xmlResponce = @$"<root><header><time>{taxes.time}</time><point>{taxes.point}</point><skey>{taxes.skey}</skey></header><operation><service>{taxes.service}</service><account><value><persacc>{taxes.persacc}</persacc></value></account></operation></root>";


                var fullUrl = url + xmlResponce;
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
                    TempData["persacc"] = taxes.persacc;
                    TempData["sum"] = taxes.sum;
                    TempData["comment"] = taxes.comment;
                    TempData["accountName"] = taxes.accountName = dictionary["fio"]["value"];
                    //taxes.accountName = dictionary["fio"]["value"];
                    globalTaxes = taxes;
                    //return View("Pay", taxes);
                    return RedirectToAction("Pay");
                }

                return View("Check");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return View();
        }

        public async Task<IActionResult> Pay(Taxes taxes, string abs = "abs")
        {
            //if (taxes.persacc == null)
            //{
            //    taxes.time = (DateTime)TempData["time"];
            //    taxes.point = int.Parse(TempData["point"].ToString());
            //    taxes.skey = TempData["skey"].ToString();
            //    taxes.service = int.Parse(TempData["service"].ToString());
            //    taxes.persacc = TempData["persacc"].ToString();
            //    taxes.accountName = TempData["accountName"].ToString();
            //    taxes.sum = int.Parse(TempData["sum"].ToString());
            //    taxes.comment = TempData["comment"].ToString();
               
            //}
            taxes = globalTaxes;

            var states = XDocument.Load("Files/States.xml");
            var districts = XDocument.Load("Files/Districts.xml");

            var statesList = new Dictionary<string, string>();
            var districtsList = new Dictionary<string, Dictionary<string, string>>();

            foreach (XElement element in states.Elements())
            {
                var list = new List<List<string>>();
                GetDataFromExcelXml(element, list);
                foreach(var l in list)
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

            return View("Pay",taxes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayBAIP([Bind("carNumber")] Taxes taxes, string Combo1, string Combo2, string Combo3)
        {
            var carNumber = taxes.carNumber;
            var states = Request;
            return View(taxes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetLocations(string locationName, string locationCode)
        {
            try {
                var location = XDocument.Load($"Files/{locationName}.xml");
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

    }
}
