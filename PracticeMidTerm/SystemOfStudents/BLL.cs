using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    internal class Programs
    {
        internal static int UpdatePrograms()
        {
            // =========================================================================
            //  Business rules for Programs
            // =========================================================================

            return Data.Programs.UpdatePrograms();
        }
    }

    internal class Students
    {
        internal static int UpdateStudents()
        {
            // =========================================================================
            //  Business rules for Students
            // =========================================================================
            /* 
            Include in your solution the business rules that no student can have year of enrolment
            neither less than 2017 nor greater than the current year(use C# "DateTime.Now.Year"
            to get the current year as an integer). Make sure to produce specific messages
            */
            
            DataTable dt = Data.Students.GetStudents()
                              .GetChanges(DataRowState.Added | DataRowState.Modified);
            if ((dt != null) && (dt.Select("YearEnrolment < 2017").Length > 0))
            {
                SystemOfStudents.Form1.msgCommandTooLow();// esse objeto e o nome do namespace no program.cs
                Data.Students.GetStudents().RejectChanges();
                return -1;
            }
            else if ((dt != null) && (dt.Select($"YearEnrolment > {DateTime.Now.Year}").Length > 0))
            {
                SystemOfStudents.Form1.msgCommandTooLow();
                Data.Students.GetStudents().RejectChanges();
                return -1;
            }
            else
            {
                return Data.Students.UpdateStudents();
            }
        }
    }
}
