using System.ComponentModel.DataAnnotations;

namespace gigadr3w.msauthflow.backoffice.api.Requests
{
    public class AddItemRequest
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required, Range(1, int.MaxValue)]
        public int Value { get; set; }
    }
}
