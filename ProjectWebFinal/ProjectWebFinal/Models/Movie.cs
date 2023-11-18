using System.ComponentModel.DataAnnotations;

namespace ProjectWebApiFinal.Models {
    public class Movie {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Poster { get; set; }
        public string Year { get; set; }

    }
}
