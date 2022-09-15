using GlobalShopping.Data.Entities;
using GlobalShopping.Enums;
using GlobalShopping.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GlobalShopping.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;

        public SeedDb(DataContext context, IUserHelper userHelper, IBlobHelper blobHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCategoriesAsync();
            await CheckProductsAsync();
            await CheckCountryAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Gerald", "Then", "gthen@yopmail.com", "809-993-2897", "Res. La Mañosa ", "Gerald.png", UserType.Admin);
            await CheckUserAsync("4040", "Admin", "Admin", "admin@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "Admin.png", UserType.Admin);
            await CheckUserAsync("2020", "Adris", "Polanco", "apolanco@yopmail.com", "849-852-2897", "Res. La Mañosa ", "Adris.png", UserType.User);
            await CheckUserAsync("3030", "Usuario", "Anonimo", "anonimo@yopmail.com", "809-222-3232", "Res. La Mañosa ", "Anonimo.png", UserType.User);
        }

        private async Task CheckProductsAsync()
        {
            if (!_context.Products.Any())
            {
                await AddProductAsync("Biberon Angie", "Frasco Redondo de Plastico, Usado para Amamantar artificialmente", 85M, 567F, new List<string>() { "Bebe" }, new List<string>() { "BiberonAngie.png" });
                await AddProductAsync("COLITAS", "Toallas Humedas Para tu bebé, dale la limpieza y cuidado que él merece", 65.01M, 232F, new List<string>() { "Bebe" }, new List<string>() { "COLITAS.png" });
                await AddProductAsync("Brillantina Dixie en Crema 4 Oz", "Crema hidratadora para peinar", 75M, 134F, new List<string>() { "Belleza" }, new List<string>() { "Brillantina.png" });
                await AddProductAsync("Shampoo De Potecito Edrys", "Es un producto que se utiliza para la limpieza y cuidado del pelo", 300M, 90F, new List<string>() { "Belleza" }, new List<string>() { "Rinse-y-Shampoo-Bona-Plus.png" });
                await AddProductAsync("Listerine 250 Ml", "Uso diario como complemento de la higiene oral", 200M, 265F, new List<string>() { "Cuidado Personal" }, new List<string>() { "Listerine.png" });
                await AddProductAsync("Colgate Roja 226ml (Gr)", "Sirve par remover las manchas de los dientes", 170M, 97F, new List<string>() { "Cuidado Personal" }, new List<string>() { "Colgate.png" });
                await AddProductAsync("Extencion Electrica 20 pie(ft)", "Sirve par remover las manchas de los dientes", 170M, 97F, new List<string>() { "Ferreteria" }, new List<string>() { "Extension.png" });
                await AddProductAsync("Tape Truper Peque 8/1", "Adhesivo utilzado para cubrir superficies de cables.", 180.01M, 410F, new List<string>() { "Ferreteria" }, new List<string>() { "Tape1.png","Tape2.png" });
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddProductAsync(string name,string description, decimal price, float stock, List<string> categories, List<string> images)
        {
            Product prodcut = new()
            {
                Description = description,
                Name = name,
                Price = price,
                Stock = stock,
                ProductCategories = new List<ProductCategory>(),
                ProductImages = new List<ProductImage>()
            };

            foreach (string? category in categories)
            {
                prodcut.ProductCategories.Add(new ProductCategory { Category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == category) });
            }


            foreach (string? image in images)
            {
                Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\products\\{image}", "products");
                prodcut.ProductImages.Add(new ProductImage { ImageId = imageId });
            }

            _context.Products.Add(prodcut);
        }



        private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, string image, UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                Guid imageId = await _blobHelper.UploadBlobAsync($"{Environment.CurrentDirectory}\\wwwroot\\images\\users\\{image}", "users");
                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phone,
                    Address = address,
                    Document = document,
                    City = _context.Cities.FirstOrDefault(),
                    UserType = userType,
                    ImageId = imageId
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());

                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);
            }
            return user;
        }


        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckCountryAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "República Dominicana",
                    States = new List<State>()
                    {
                        new State
                        {
                            Name= "Santo Domingo",
                            Cities = new List<City>()
                            {
                                new City {Name ="Santo Domingo Este"},
                                new City {Name ="Santo Domingo Norte"},
                                new City {Name ="Santo Domingo Oeste"},
                                new City {Name ="Boca Chica"},
                                new City {Name ="Los Alcarrizos"},
                                new City {Name ="Pedro Brand"},
                                new City {Name ="San Antonio de Guerra"},
                                new City {Name ="San Luis"},
                            }
                        },
                        new State
                        {
                            Name= "Santiago",
                            Cities = new List<City>()
                            {
                                new City {Name ="Santiago de los Caballeros"},
                                new City {Name ="Licey"},
                                new City {Name ="Tamboril"},
                                new City {Name ="Puñal"},
                                new City {Name ="Villa González"},
                                new City {Name ="Villa Bisonó"},
                                new City {Name ="San José de las Matas"},
                                new City {Name ="Sabana Iglesia"},
                                new City {Name ="Jánico"},
                            }
                        },
                    },

                });

                _context.Countries.Add(new Country
                {
                    Name = "Colombia",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Antioquia",
                            Cities = new List<City>() {
                                new City() { Name = "Medellín" },
                                new City() { Name = "Itagüí" },
                                new City() { Name = "Envigado" },
                                new City() { Name = "Bello" },
                                new City() { Name = "Rionegro" },
                            }
                        },
                        new State()
                        {
                            Name = "Bogotá",
                            Cities = new List<City>() {
                                new City() { Name = "Usaquen" },
                                new City() { Name = "Champinero" },
                                new City() { Name = "Santa fe" },
                                new City() { Name = "Useme" },
                                new City() { Name = "Bosa" },
                            }
                        },
                        new State()
                        {
                            Name = "Valle",
                            Cities = new List<City>() {
                                new City() { Name = "Calí" },
                                new City() { Name = "Jumbo" },
                                new City() { Name = "Jamundí" },
                                new City() { Name = "Chipichape" },
                                new City() { Name = "Buenaventura" },
                                new City() { Name = "Cartago" },
                                new City() { Name = "Buga" },
                                new City() { Name = "Palmira" },
                            }
                        },
                        new State()
                        {
                            Name = "Santander",
                            Cities = new List<City>() {
                                new City() { Name = "Bucaramanga" },
                                new City() { Name = "Málaga" },
                                new City() { Name = "Barrancabermeja" },
                                new City() { Name = "Rionegro" },
                                new City() { Name = "Barichara" },
                                new City() { Name = "Zapatoca" },
                            }
                        },

                    }
                });

                _context.Countries.Add(new Country
                {
                    Name = "Estados Unidos",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Florida",
                            Cities = new List<City>() {
                                new City() { Name = "Orlando" },
                                new City() { Name = "Miami" },
                                new City() { Name = "Tampa" },
                                new City() { Name = "Fort Lauderdale" },
                                new City() { Name = "Key West" },
                            }
                        },
                        new State()
                        {
                            Name = "Texas",
                            Cities = new List<City>() {
                                new City() { Name = "Houston" },
                                new City() { Name = "San Antonio" },
                                new City() { Name = "Dallas" },
                                new City() { Name = "Austin" },
                                new City() { Name = "El Paso" },
                            }
                        },
                        new State()
                        {
                            Name = "California",
                            Cities = new List<City>() {
                                new City() { Name = "Los Angeles" },
                                new City() { Name = "San Francisco" },
                                new City() { Name = "San Diego" },
                                new City() { Name = "San Bruno" },
                                new City() { Name = "Sacramento" },
                                new City() { Name = "Fresno" },
                            }
                        },

                    }
                });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Bebe" });
                _context.Categories.Add(new Category { Name = "Belleza" });
                _context.Categories.Add(new Category { Name = "Cuidado Personal" });
                _context.Categories.Add(new Category { Name = "Ferreteria" });
                _context.Categories.Add(new Category { Name = "Hogar" });
                _context.Categories.Add(new Category { Name = "Juguetes" });
                _context.Categories.Add(new Category { Name = "Limpieza" });
                _context.Categories.Add(new Category { Name = "Medicamentos" });
                _context.Categories.Add(new Category { Name = "Misceláneos" });
                _context.Categories.Add(new Category { Name = "Oficina" });
                _context.Categories.Add(new Category { Name = "Plásticos" });
            }

            await _context.SaveChangesAsync();
        }

    }
}
