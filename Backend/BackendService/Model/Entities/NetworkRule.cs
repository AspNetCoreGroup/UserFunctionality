using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendService.Model.Entities
{
    [Table("NetworkRules")]
    public class NetworkRule
    {
        [Key]
        public int NetworkRuleID { get; set; }

        public int NetworkID { get; set; }

        public int? UserID { get; set; }

        public string? NotificationType { get; set; }

        public string? RuleExpression { get; set; }

        public Network? Network { get; set; }

        public User? User { get; set; }
    }
}