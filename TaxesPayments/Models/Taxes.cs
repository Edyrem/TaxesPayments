using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TaxesPayments.Models
{
    public class Taxes
    {
        public int id { get; set; }

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

        // ключ на аппарат (для валидации ответа от сервера)
        public string? akey { get; set; }

        // код ошибки (0 — успех, 1 — ошибка)
        public int err { get; set; }

        // описание ошибки
        public string? msg { get; set; }

        // сумма платежа
        public double? add { get; set; }

        // комментарий
        public string? comment { get; set; }

        // сумма
        public int sum { get; set; }

        // номер машины для услуги 1520
        public string? carNumber { get; set; }
    }
}
