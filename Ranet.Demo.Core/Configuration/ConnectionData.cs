using Ranet.Olap.Core.Storage;

namespace Ranet.Demo.Core.Configuration
{
    public class ConnectionData
    {
        public string ConnectionStringId { get; set; }
        public string ConnectionStringOlap { get; set; }

        public ConnectionStringStorage Storage { get; set; }

        public ConnectionData()
        {
        }

        public ConnectionData(string id, string olap, ConnectionStringStorage storage)
        {
            ConnectionStringId = id;
            ConnectionStringOlap = olap;
            Storage = storage;
        }

        public override bool Equals(object obj)
        {
            if (obj == null && GetType() != obj.GetType())
                return false;

            ConnectionData old = (ConnectionData) obj;

            return (this.ConnectionStringId == old.ConnectionStringId) &&
                   (this.ConnectionStringOlap == old.ConnectionStringOlap) && (Storage.Equals(old.Storage));
        }
    }

}