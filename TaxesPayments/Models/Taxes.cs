using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TaxesPayments.Models
{
    public class Taxes
    {
        public int id { get; set; }

        /* QuickPay options */
        // Id-шестизначное число для отправки в тело запроса QuickPay, генерируется. Должно быть уникальным
        public int quickPayId { get; set; }

        // время приема платежа (Формат: YYYY-mm-dd HH:MM:SS, Пример: 2011-03-03 09:36:26)
        public DateTime time { get; set; }

        // номер точки через которую будут проводиться платежи
        public int point { get; set; }

        // ключ на сервер (для валидации запроса на сервер)
        public string? skey { get; set; }

        // номер сервиса
        public int service { get; set; }

        // тэг для передачи реквизитов абонента
        public int account { get; set; }

        // идентификатор для оплаты (номер телефона, номер лицевого счета и т.д.)
        public string? persacc { get; set; }

        // ФИО абонента
        public string? accountName { get; set; }

        // ошибка QuickPay, если таковая будет, иначе пусто
        public string? errMessage { get; set; }

        // комментарий
        public string? accountComment { get; set; }

        // сумма
        public int sum { get; set; }

        // номер машины для услуги 1520
        public string? carNumber { get; set; }

        /* BAIP Options */
        // комиссия за услугу БАИП
        public int sum_usluga { get; set; }

        // статус БАИПа
        public string? baip_status { get; set; }

        // сообщение от БАИПа
        public string? baip_message { get; set; }

        public string? baip_cashregister_id { get; set; }

        public string? baip_receipt_id { get; set; }

        public string? baip_partner_id { get; set; }

        public string? baip_serviceNumber { get; set; }
    }
}
