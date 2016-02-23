using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Candlesticks
{
	[DataContract]
	[KnownType(typeof(OrderBookUpdated))]
	public class ServerEvent {
		[DataMember]
		public object Body;
	}


	[DataContract]
    public class OrderBookUpdated : ServerEvent {
		[DataMember]
		public DateTime DateTime;
    }
}
