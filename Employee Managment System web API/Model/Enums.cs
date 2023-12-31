using System.Runtime.Serialization;

namespace Employee_Management_System.Model
{
    public enum AttendanceStatus // move to enum folder in model
    {
        /*[EnumMember(Value = "Present")]*/
        Present,
        Absent,
        Vacation,
        Remote
    }
    public enum LeaveStatus
    {
        Pending,
        Approved,
        Denied
    }
    /*
    public string GetEnumMemberValue(Enum value)
    {
        try
        {
            var memberInfo = value.GetType().GetMember(value.ToString());
            var enumMemberAttr = memberInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false).FirstOrDefault() as EnumMemberAttribute;
            return enumMemberAttr?.Value ?? value.ToString();
        }
        catch(Exception e)
        {
            throw e;
        }
    }
     */
}
