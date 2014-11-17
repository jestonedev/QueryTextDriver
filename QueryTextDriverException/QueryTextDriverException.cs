using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace QueryTextDriverExceptionNS
{
    [Serializable()]
    public class QueryTextDriverException : Exception
    {
        /// <summary>
        /// Конструктор класса исключения QueryTextDriverException
        /// </summary>
        public QueryTextDriverException() : base() { }

        /// <summary>
        /// Конструктор класса исключения QueryTextDriverException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        public QueryTextDriverException(string message) : base(message) { }

        /// <summary>
        /// Конструктор класса исключения QueryTextDriverException
        /// </summary>
        /// <param name="message">Сообщение об ошибке</param>
        /// <param name="innerException">Вложенное исключение</param>
        public QueryTextDriverException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Конструктор класса исключения QueryTextDriverException
        /// </summary>
        /// <param name="info">Информация сериализации</param>
        /// <param name="context">Контекст потока</param>
        protected QueryTextDriverException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
