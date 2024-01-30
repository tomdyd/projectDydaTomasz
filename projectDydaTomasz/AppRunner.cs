using projectDydaTomasz.Core.Interfaces;
using projectDydaTomasz.Core.Models;
using projectDydaTomasz.Core.Services;
using projectDydaTomasz.Interfaces;
using projectDydaTomaszCore.Models;
using System.Runtime.CompilerServices;

namespace projectDydaTomasz
{
    public class AppRunner
    {
        private readonly IMenu _menu;
        private readonly IAppConsole _console;
        private readonly IDatabaseConnectionExtended<User> _userMongoClient;
        private readonly IDatabaseConnectionExtended<Car> _carMongoClient;
        private readonly IDatabaseConnectionExtended<Apartment> _apartmentMongoClient;
        private readonly IUserService _userMongoService;
        private readonly ICarService _carMongoService;
        private readonly IApartmentService _apartmentMongoService;
        private readonly IUserService _userSqlService;
        private readonly ICarService _carSqlService;
        private readonly IApartmentService? _apartmentSqlService;

        public AppRunner(
            IMenu menu,
            IAppConsole console,
            IDatabaseConnectionExtended<User> userMongoClient,
            IDatabaseConnectionExtended<Car> carMongoClient,
            IDatabaseConnectionExtended<Apartment> apartmentMongoClient,
            IUserService userMongoService,
            ICarService carMongoService,
            IApartmentService apartmentMongoService,
            IUserService userSqlService,
            ICarService carSqlService,
            IApartmentService apartmentSqlService)
        {
            _menu = menu;
            _console = console;
            _userMongoClient = userMongoClient;
            _carMongoClient = carMongoClient;
            _apartmentMongoClient = apartmentMongoClient;
            _userMongoService = userMongoService;
            _carMongoService = carMongoService;
            _apartmentMongoService = apartmentMongoService;
            _userSqlService = userSqlService;
            _carSqlService = carSqlService;
            _apartmentSqlService = apartmentSqlService;
        }

        public void StartApp()
        {
            while (true)
            {
                _console.Clear();
                _menu.MainMenu();
                var res = _console.GetResponseFromUser();

                switch (res)
                {
                    case 1:
                        _userMongoClient.Connect("mongodb://localhost:27017/", "dataBase", "user");
                        _carMongoClient.Connect("mongodb://localhost:27017/", "dataBase", "car");
                        _apartmentMongoClient.Connect("mongodb://localhost:27017/", "dataBase", "apartment");

                        StartClient(MongoCollectionsMenu, _userMongoService);
                        
                        break;

                    case 2:

                        StartClient(SqlCollectionsMenu, _userSqlService);

                        break;

                    case 3:
                        return;

                    default:
                        _console.WriteLine("Nie ma takiej opcji");
                        _console.ReadLine();
                        break;
                }
            }
        }

        private delegate void CollectionsMenuDelegate(bool runCollectionsMenu, User loggedUser);

        private void MongoCollectionsMenu(bool runCollectionsMenu, User loggedUser)
        {
            while (runCollectionsMenu)
            {
                _console.Clear();
                _menu.CollectionsMenu();
                var res = _console.GetResponseFromUser();

                switch (res)
                {
                    case 1:
                        bool runCarMenu = true;
                        _userMongoClient.Connect("mongodb://localhost:27017/", "dataBase", "user");
                        MongoCarMenu(runCarMenu, loggedUser);

                        break;

                    case 2:
                        bool runApartmentMenu = true;
                        _userMongoClient.Connect("mongodb://localhost:27017/", "dataBase", "user");
                        MongoApartmentsMenu(runApartmentMenu, loggedUser);

                        break;

                    case 3:
                        runCollectionsMenu = false;
                        loggedUser = null;
                        break;

                    default:
                        _console.WriteLine("Nie ma takiej opcji");
                        break;
                }
            }
        }        

        private void MongoCarMenu(bool runCarsMenu, User loggedUser)
        {
            while (runCarsMenu)
            {
                _console.Clear();
                _menu.carMenu();
                var res = _console.GetResponseFromUser();

                switch (res)
                {
                    case 1:

                        CreateCar(loggedUser, _carMongoService);
                        break;

                    case 2:
                        var carList = _carMongoService.GetCars("user", loggedUser.userId);
                        PrintList(carList);

                        break;

                    case 3:
                        carList = _carMongoService.GetCars("user", loggedUser.userId);
                        PrintFilteredCarsList(carList);

                        break;

                    case 4:
                        carList = _carMongoService.GetCars("user", loggedUser.userId);
                        UpdateCar(loggedUser, carList, _carMongoService);

                        break;

                    case 5:
                        carList = _carMongoService.GetCars("user", loggedUser.userId);
                        DeleteCar(carList, _carMongoService);

                        break;

                    case 6:
                        runCarsMenu = false;
                        break;

                    default:
                        _console.WriteLine("Nie ma takiej opcji");
                        break;
                }
            }
        }

        private void MongoApartmentsMenu(bool runApartmentsMenu, User loggedUser)
        {
            while (runApartmentsMenu)
            {
                _console.Clear();
                _menu.apartmentMenu();
                var res = _console.GetResponseFromUser();

                switch (res)
                {
                    case 1:

                        CreateApartment(loggedUser, _apartmentMongoService);

                        break;

                    case 2:
                        var apartmentsList = _apartmentMongoService.GetApartments("user", loggedUser.userId);
                        PrintList(apartmentsList);

                        break;

                    case 3:

                        apartmentsList = _apartmentMongoService.GetApartments("user", loggedUser.userId);
                        UpdateApartment(loggedUser, apartmentsList, _apartmentMongoService);

                        break;

                    case 4:

                        apartmentsList = _apartmentMongoService.GetApartments("user", loggedUser.userId);
                        DeleteApartment(apartmentsList, _apartmentMongoService);

                        break;

                    case 5:
                        runApartmentsMenu = false;
                        break;

                    default:
                        _console.WriteLine("Nie ma takiej opcji!");
                        _console.ReadLine();
                        break;
                }
            }
        }

        private void SqlCollectionsMenu(bool runCollectionsMenu, User loggedUser)
        {
            while (runCollectionsMenu)
            {
                _console.Clear();
                _menu.CollectionsMenu();
                var res = _console.GetResponseFromUser();

                switch (res)
                {
                    case 1:

                        bool runCarMenu = true;
                        SqlCarsMenu(runCarMenu, loggedUser);

                        break;

                    case 2:

                        bool runApartmentMenu = true;
                        SqlApartmentsMenu(runApartmentMenu, loggedUser);

                        break;

                    case 3:
                        runCollectionsMenu = false;
                        loggedUser = null;
                        break;

                    default:
                        _console.WriteLine("Nie ma takiej opcji!");
                        _console.ReadLine();
                        break;
                }
            }
        }

        private void SqlCarsMenu(bool runCarsMenu, User loggedUser)
        {
            while (runCarsMenu)
            {
                _console.Clear();
                _menu.carMenu();
                var res = _console.GetResponseFromUser();

                switch (res)
                {
                    case 1:
                        CreateCar(loggedUser, _carSqlService);

                        break;

                    case 2:
                        var carList = _carSqlService.GetCars("SELECT * FROM Cars", $"WHERE user = '{loggedUser.userId}'");
                        PrintList(carList);

                        break;

                    case 3:

                        carList = _carSqlService.GetCars("SELECT * FROM Cars", $"WHERE user = '{loggedUser.userId}'");
                        PrintFilteredCarsList(carList);

                        break;

                    case 4:

                        carList = _carSqlService.GetCars("SELECT * FROM Cars", $"WHERE user = '{loggedUser.userId}'");
                        UpdateCar(loggedUser, carList, _carSqlService);

                        break;

                    case 5:
                        carList = _carSqlService.GetCars("SELECT * FROM Cars", $"WHERE user = '{loggedUser.userId}'");
                        DeleteCar(carList, _carSqlService);

                        break;

                    case 6:
                        runCarsMenu = false;
                        break;

                    default:
                        _console.WriteLine("Nie ma takiej opcji!");
                        _console.ReadLine();
                        break;
                }
            }
        }

        private void SqlApartmentsMenu(bool runApartmentsMenu, User loggedUser)
        {
            while (runApartmentsMenu)
            {
                _console.Clear();
                _menu.apartmentMenu();
                var res = _console.GetResponseFromUser();

                switch (res)
                {
                    case 1:
                        CreateApartment(loggedUser, _apartmentSqlService);
                        break;

                    case 2:

                        var apartmentsList = _apartmentSqlService.GetApartments("SELECT * FROM Apartments", $"WHERE user = '{loggedUser.userId}'");
                        PrintList(apartmentsList);

                        break;

                    case 3:
                        apartmentsList = _apartmentSqlService.GetApartments("SELECT * FROM Apartments", $"WHERE user = '{loggedUser.userId}'");
                        UpdateApartment(loggedUser, apartmentsList, _apartmentSqlService);

                        break;

                    case 4:

                        apartmentsList = _apartmentSqlService.GetApartments("SELECT * FROM Apartments", $"WHERE user = '{loggedUser.userId}'");
                        DeleteApartment(apartmentsList, _apartmentSqlService);

                        break;

                    case 5:
                        runApartmentsMenu = false;
                        break;

                    default:
                        _console.WriteLine("Nie ma takiej opcji!");
                        break;
                }
            }
        }

        private void CreateUser(IUserService userService)
        {
            var newUser = new User()
            {
                username = _console.GetDataFromUser("Podaj login: "),
                passwordHash = _console.GetPasswordFromUser(),
                email = _console.GetDataFromUser("Podaj adres email: ")
            };

            userService.RegisterUser(newUser);
        }

        private void CreateCar(User loggedUser, ICarService carService)
        {
            try
            {
                Car newCar = new Car();

                GetCarDataFromUser(loggedUser, newCar);
                carService.CreateCar(newCar);
                _console.WriteLine("Dane zostały zapisane w bazie danych.");

                _console.ReadLine();
            }
            catch (Exception e)
            {
                _console.WriteLine(e.Message);
                _console.ReadLine();
            };
        }

        private void CreateApartment(User loggedUser, IApartmentService apartmentService)
        {
            try
            {
                var newApartment = new Apartment();

                GetApartmentDataFromUser(loggedUser, newApartment);

                apartmentService.CreateApartment(newApartment);
                _console.WriteLine("Dane zostały zapisane w bazie danych.");
                _console.ReadLine();
            }
            catch (Exception e)
            {
                _console.WriteLine(e.Message);
                _console.ReadLine();
            };
        }

        private void Print<T>(List<T> dataList)
        {
            foreach (T item in dataList)
            {
                _console.Write($"{dataList.IndexOf(item) + 1}. ");
                _console.WriteLine(item.ToString());
            }
        }

        private void PrintList<T>(List<T> dataList)
        {
            try
            {
                Print(dataList);

                _console.ReadLine();
            }
            catch (Exception e)
            {
                _console.WriteLine(e.Message);
                _console.ReadLine();
            }
        }

        private void PrintFilteredCarsList(List<Car> dataList)
        {
            try
            {
                var searchTerm = _console.GetDataFromUser("Podaj marke szukanego samochodu: ");
                List<Car> carList = new List<Car>();

                foreach (var item in dataList)
                {
                    if (item.carBrand == searchTerm)
                    {
                        carList.Add(item);
                    }
                }

                Print(carList);

                _console.ReadLine();
            }
            catch (Exception e)
            {
                _console.WriteLine(e.Message);
                _console.ReadLine();
            }
        }        

        private void UpdateCar(User loggedUser, List<Car> carsList, ICarService carService)
        {
            try
            {
                Print(carsList);

                _console.Write("Podaj numer samochodu który chcesz zaktualizować: ");
                var carNumber = _console.GetResponseFromUser();

                if (carNumber <= carsList.Count)
                {
                    var updatingCar = carsList[carNumber - 1];

                    GetCarDataFromUser(loggedUser, updatingCar);

                    carService.UpdateCar(updatingCar);

                    _console.WriteLine("Dane zaktualizowane!");
                    _console.ReadLine();
                }
                else
                {
                    _console.WriteLine("Nie znaleziono samochodu!");
                    _console.ReadLine();
                }
            }
            catch (Exception e)
            {
                _console.WriteLine(e.Message);
                _console.ReadLine();
            }
        }

        private void UpdateApartment(User loggedUser, List<Apartment> apartmentsList, IApartmentService apartmentService)
        {
            try
            {
                Print(apartmentsList);

                _console.Write("Podaj numer mieszkania które chcesz zaktualizować: ");
                var apartmentNumber = _console.GetResponseFromUser();

                if (apartmentNumber <= apartmentsList.Count)
                {
                    var updatingApartment = apartmentsList[apartmentNumber - 1];

                    GetApartmentDataFromUser(loggedUser, updatingApartment);

                    apartmentService.UpdateApartment(updatingApartment);

                    _console.WriteLine("Dane zaktualizowane!");
                    _console.ReadLine();
                }
                else
                {
                    _console.WriteLine("Nie znaleziono mieszkania!");
                    _console.ReadLine();
                }
            }
            catch (Exception e)
            {
                _console.WriteLine(e.Message);
                _console.ReadLine();
            }
        }

        private void DeleteCar(List<Car> carList, ICarService carService)
        {
            try
            {
                Print(carList);

                _console.Write("Podaj numer samochodu który chcesz usunąć: ");
                var carNumber = _console.GetResponseFromUser();

                if (carNumber <= carList.Count)
                {
                    var deletingCar = carList[carNumber - 1];

                    carService.DeleteCar(deletingCar.carId);
                    _console.WriteLine("Samochód został usunięty!");
                    _console.ReadLine();
                }
                else
                {
                    _console.WriteLine("Nie znaleziono samochodu!");
                    _console.ReadLine();
                }
            }
            catch (Exception e)
            {
                _console.WriteLine(e.Message);
                _console.ReadLine();
            }
        }

        private void DeleteApartment(List<Apartment> apartmentsList, IApartmentService apartmentService)
        {
            try
            {
                Print(apartmentsList);

                _console.Write("Podaj numer mieszkania które chcesz usunąć: ");

                var apartmentNumber = _console.GetResponseFromUser();

                if (apartmentNumber <= apartmentsList.Count)
                {
                    var deletingApartment = apartmentsList[apartmentNumber - 1];

                    apartmentService.DeleteApartment(deletingApartment.apartmentId);

                    _console.WriteLine("Mieszkanie zostało usunięte!");
                    _console.ReadLine();
                }
                else
                {
                    _console.WriteLine("Nie znaleziono mieszkania!");
                    _console.ReadLine();
                }
            }
            catch (Exception e)
            {
                _console.WriteLine(e.Message);
                _console.ReadLine();
            }
        }

        private Car GetCarDataFromUser(User loggedUser, Car car)
        {
            if (car.carId != null)
            {
                car.carId = car.carId;
            }
            car.carBrand = _console.GetDataFromUser("Podaj markę samochodu: ");
            car.carModel = _console.GetDataFromUser("Podaj model samochodu: ");
            car.carProductionYear = _console.GetDataFromUser("Podaj rok produkcji: ");
            car.engineCapacity = _console.GetDataFromUser("Podaj pojemność silnika: ");
            car.user = loggedUser.userId;

            return car;
        }

        private Apartment GetApartmentDataFromUser(User loggedUser, Apartment apartment)
        {
            if (apartment.apartmentId != null)
            {
                apartment.apartmentId = apartment.apartmentId;
            }
            apartment.surface = _console.GetDataFromUser("Podaj powierzchnię mieszkania: ");
            apartment.street = _console.GetDataFromUser("Podaj adres mieszkania: ");
            apartment.cost = _console.GetDataFromUser("Podaj cenę mieszkania: ");
            apartment.user = loggedUser.userId;

            return apartment;
        }        

        private void GetUserData(CollectionsMenuDelegate collectionsMenuDelegate, IUserService userService)
        {
            _console.Clear();
            
            var login = _console.GetLoginFromUser();
            var password = _console.GetPasswordFromUser();
            var loggedUser = userService.AuthorizeUser(login, password);

            if (loggedUser != null)
            {
                bool runCollectionsMenu = true;

                collectionsMenuDelegate(runCollectionsMenu, loggedUser);
            }
        }

        private void StartClient(CollectionsMenuDelegate collectionsMenuDelegate, IUserService userService)
        {
            var runLoginMenu = true;
            while (runLoginMenu)
            {
                _console.Clear();
                _menu.LoginMenu();
                var res = _console.GetResponseFromUser();

                switch (res)
                {
                    case 1:

                        GetUserData(collectionsMenuDelegate, userService);

                        break;

                    case 2:
                        CreateUser(userService);

                        break;

                    case 3:
                        runLoginMenu = false;
                        break;

                    default:
                        _console.WriteLine("Nie ma takiej opcji!");
                        break;
                }

            }
        }

    }
}
