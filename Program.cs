using EwidencjaSprzetuOOP.Config;
using EwidencjaSprzetuOOP.Data;
using EwidencjaSprzetuOOP.Data.Repositories;
using EwidencjaSprzetuOOP.Services;
using EwidencjaSprzetuOOP.UI;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var cfg = AppConfig.Load();

var scriptPath = Path.Combine(AppContext.BaseDirectory, "db", "EwidencjaSprzetuDb.sql");
DbBootstrapper.EnsureDatabaseExists(cfg.ConnectionStrings.Default, scriptPath);

var factory = new SqlConnectionFactory(cfg.ConnectionStrings.Default);

var dzialRepo = new DzialRepository(factory);
var lokRepo = new LokalizacjaRepository(factory);
var dostRepo = new DostawcaRepository(factory);

var pracRepo = new PracownikRepository(factory);
var sprzetRepo = new SprzetRepository(factory);
var przyRepo = new PrzypisanieRepository(factory);
var serwisRepo = new SerwisRepository(factory);

var sprzetService = new SprzetService(sprzetRepo);
var pracService = new PracownikService(pracRepo);
var przyService = new PrzypisanieService(przyRepo, sprzetRepo);
var serwisService = new SerwisService(serwisRepo, sprzetRepo);
var raporty = new RaportyService(factory);
var slowniki = new SlownikiService(dzialRepo, lokRepo, dostRepo);

var export = new ExportService(sprzetService, pracService, przyService, serwisService, raporty);
var import = new ImportService(sprzetService, pracService, slowniki, serwisService, przyService);

var ui = new ConsoleUi(sprzetService, pracService, przyService, serwisService, raporty, slowniki, export, import);
ui.Run();
