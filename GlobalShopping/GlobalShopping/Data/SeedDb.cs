using GlobalShopping.Data.Entities;
using GlobalShopping.Enums;
using GlobalShopping.Helpers;

namespace GlobalShopping.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCategoriesAsync();
            await CheckCountryAsync();
            await CheckRolesAsync();
            await CheckUserAsync("1010", "Gerald", "Then", "gthen@yopmail.com", "809-993-2897", "Res. La Mañosa ", UserType.Admin);
            await CheckUserAsync("2020", "Adris", "Polanco", "apolanco@yopmail.com", "849-852-2897", "Res. La Mañosa ", UserType.User);
        }

        private async Task<User> CheckUserAsync(string document,string firstName,string lastName,string email,string phone,string address, UserType userType)
        {
            User user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
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
                };

                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUserToRoleAsync(user, userType.ToString());
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
                    }
                });
                await _context.SaveChangesAsync();
            }
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Tecnología" });
                _context.Categories.Add(new Category { Name = "Ropa" });
                _context.Categories.Add(new Category { Name = "Calzado" });
                _context.Categories.Add(new Category { Name = "Belleza" });
                _context.Categories.Add(new Category { Name = "Deporte" });
                _context.Categories.Add(new Category { Name = "Nutrición" });
                _context.Categories.Add(new Category { Name = "Apple" });
                _context.Categories.Add(new Category { Name = "Mascostas" });
                await _context.SaveChangesAsync();
            }
                
        }
    }
}
