using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TB.Entities
{
    public enum Roles
    {
        /// <summary>
        /// Administrator
        /// </summary>
        [Description("Administrator")]
        Administrator = 0,

        /// <summary>
        /// Employee
        /// </summary>
        [Description("Employee")]
        Employee = 1,
    }
}
