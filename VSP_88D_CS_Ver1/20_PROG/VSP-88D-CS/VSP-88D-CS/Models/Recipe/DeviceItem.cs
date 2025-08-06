using System.ComponentModel.DataAnnotations.Schema;
using VSLibrary.Database;

namespace VSP_88D_CS.Models.Recipe
{
    /// <summary>
    /// A data model class that represents data in the Device table.
    /// </summary>
    [Table(nameof(DeviceItem))] // 선택 사항: 테이블 이름 지정
    public class DeviceItem
    {
        /// <summary>
        /// Unique ID for the device record.
        /// </summary>
        //[PrimaryKey]
        //public int Id { get; set; }

        //Switch datatype for using VSLibrary
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// The name of the device.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The date the device record was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// The date the device record was modified.
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// The name of the Recipe associated with the Device.
        /// </summary>
        //public string RecipeName { get; set; }

        /// <summary>
        /// The name of the worker who created the device.
        /// </summary>
        public string Operator { get; set; }
    }
}
