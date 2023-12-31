using Employee_Management_System.Model;

namespace Employee_Management_System.DAL
{
    public class DCommitDBChanges : IDCommitDBChanges
    {
        private readonly AppDbContext _contest;
        public DCommitDBChanges(AppDbContext contest)
        {
            _contest = contest;
        }
        public void SaveChanges()
        {
            _contest.SaveChanges();
        }
    }
}
