using System.Text;
using System.Net.Http;
using TaxesPayments.Models;
using System.Net;
using Azure.Core;
using System.Xml.Linq;
using System;
using Azure;

namespace TaxesPayments.Helpers
{
    public class BAIP
    {

        private readonly HttpClient _client;
        private string SOAPResponse = "";
        private readonly string url_ = "http://192.168.181.2/Operator.ashx";
        //private string url_ = "http://192.168.175.2/Operator.ashx";

        public BAIP(HttpClient client)
        {
            _client = client;
        }

        public string SetSoapXml(Taxes taxes, Dictionary<string, string> soapOptions, string productCode, string partnerCode, string serviceNumber)
        {
            var xml = @$"<?xml version='1.0' encoding='UTF-8'?>
                                <soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/'
                                    xmlns:xsd='http://www.w3.org/2001/XMLSchema'
                                    xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'>
                                    <soapenv:Body>
                                    <salesorder_request xmlns='' xmlns:ns1='http://tempuri.org/PartnerMsgSet/definitions'
                                    xsi:type='ns1:Msg_salesorder_soap_requestType'>
                                    <general>
                                        <partner_transaction_id>{taxes.id}</partner_transaction_id>
                                        <kiosk_id>{soapOptions["kiosk_id"]}</kiosk_id>
                                        <partner_contract_id>{serviceNumber}</partner_contract_id>
                                        <host_ip>{soapOptions["host_ip"]}</host_ip>
                                        <session_start>{soapOptions["time"]}</session_start>
                                        <cashregister_id>{soapOptions["cashregister_id"]}</cashregister_id>
                                        <receipt_id>{soapOptions["receipt_id"]}</receipt_id>
                                        <partner_id>{soapOptions["partner_id"]}</partner_id>
                                    </general>
                                    <products>
                                        <item>
                                            <partner_row_id>{taxes.id}</partner_row_id>
                                            <kiosk_productcode>{productCode}</kiosk_productcode>
                                            <partner_productcode>{partnerCode}</partner_productcode>
                                            <transaction_type>SALES</transaction_type>
                                            <quantity>1</quantity>
                                            <price>{taxes.sum}</price>
                                            <description>{taxes.persacc}{{NL}}{taxes.accountName}{{NL}}Номер транзакции:{taxes.id}</description>
                                            <partner_customer_id>{taxes.persacc}</partner_customer_id>
                                        </item>
                                        </products>
                                        </salesorder_request>
                                    </soapenv:Body>
                                </soapenv:Envelope>";
            return xml;
        }
                              
        public async Task<string> SetSoapRequest(string url, string soapEnvelope)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(soapEnvelope);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Post;
            //  request.KeepAlive = true;
            request.ContentLength = bytes.Length;
            request.ContentType = "application/xml";
            request.Accept = "application/xml";

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var responceBaip = "";
            try
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                responceBaip = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                return responceBaip;
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse)ex.Response;
                if (response != null) // timeout
                {
                    return response.StatusCode.ToString(); 
                }
            }

            return responceBaip;
        }

    }
}
