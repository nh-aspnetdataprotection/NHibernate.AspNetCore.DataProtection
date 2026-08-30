namespace NHibernate.AspNetCore.DataProtection
{
    /// <summary>
    /// Model used by <see cref="ISession"/>.
    /// </summary>
    public class DataProtectionKey
    {
        /// <summary>
        /// The entity identifier of the <see cref="DataProtectionKey"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The friendly name of the <see cref="DataProtectionKey"/>.
        /// </summary>
        public string? FriendlyName { get; set; }

        /// <summary>
        /// The XML representation of the <see cref="DataProtectionKey"/>.
        /// </summary>
        public string? Xml { get; set; }
        
        public override int GetHashCode() => Id;
        
        protected bool Equals(DataProtectionKey other) => Id == other.Id;

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DataProtectionKey)obj);
        }
    }
}
