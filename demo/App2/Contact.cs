//using NativeBuffering;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//[BufferedMessageSource]
//public partial class Contact
//{
//    public Contact(string id, string name, Address[] addresses)
//    {
//        Id = id;
//        Name = name;
//        ShipAddresses = addresses;
//    }

//    public string Id { get; }
//    public string Name { get; }
//    public IList<>
//    public Address[] ShipAddresses { get; }
//}

//[BufferedMessageSource]
//public partial class Address
//{       
//    public string Province { get; }
//    public string City { get; }
//    public string District { get; }
//    public string Street { get; }
//    public Address(string province, string city, string district, string street)
//    {
//        Province = province ?? throw new ArgumentNullException(nameof(province));
//        City = city ?? throw new ArgumentNullException(nameof(city));
//        District = district ?? throw new ArgumentNullException(nameof(district));
//        Street = street ?? throw new ArgumentNullException(nameof(street));
//    }
//}

