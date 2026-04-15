//Onde nossa classe esta organizada dentro do projeto
namespace BoschPizza.Models;

public class Pizza
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsGlutenFree { get; set; }

    
    public decimal Price {get; set;}
}


