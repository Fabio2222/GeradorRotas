using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using GeradorRotas.Frontend.Service;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;
using RouteAdministration.Frontend.Service;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;

namespace GeradorRotas.Frontend.Controllers
{
    public class RotaController : Controller
    {
        IWebHostEnvironment _appEnvironment;
        private readonly GRotasService _gRotasService;

        public RotaController(IWebHostEnvironment env, GRotasService gRotasService)
        {
            _appEnvironment = env;
            _gRotasService = gRotasService;
        }

        public IActionResult Index()
        {
            string usuario = "Anonymous";
            bool authenticate = false;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                usuario = HttpContext.User.Identity.Name;
                authenticate = true;

                if (HttpContext.User.IsInRole("adm"))
                    ViewBag.Role = "adm";
                else
                    ViewBag.Role = "user";
            }
            else
            {
                usuario = "Não Logado";
                authenticate = false;
                ViewBag.Role = "";
            }

            var headers = ReadFiles.ReadHeaderExcelFile(_appEnvironment.WebRootPath);

            ViewBag.Usario = usuario;
            ViewBag.Authenticate = authenticate;
            ViewBag.Headers = headers;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Services(List<string> selectedHeaders)
        {
            List<string> services = new();
            List<string> servicesSearch = new();

            selectedHeaders.ForEach(header =>
            {
                if (header.ToUpper() == "SERVIÇO" || header.ToUpper() == "SERVICO")
                {
                    services = ReadFiles.ReadColumnExcelFile(_appEnvironment.WebRootPath, header);
                }
            });

            services.Sort((x, y) => x.CompareTo(y));

            var servicesDuplicate = services.GroupBy(service => service.ToString()).Where(x => x.Count() > 1);

            foreach (var service in servicesDuplicate)
            {
                servicesSearch.Add(service.Key);
            }

            var a = ReadFiles.ReadExcelFile(selectedHeaders, _appEnvironment.WebRootPath);

            WriteFiles.WriteStringInFolder(selectedHeaders, null, "headers", _appEnvironment.WebRootPath);

            ViewBag.Services = servicesSearch;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Cities(string service)
        {
            var columns = ReadFiles.ReadFileInFolder("headers", _appEnvironment.WebRootPath);
            var dataBySelectedHeaders = ReadFiles.ReadExcelFile(columns, _appEnvironment.WebRootPath);

            List<string> cidades = new();
            List<IDictionary<string, string>> dictonaryByService = new();

            columns.ForEach(column =>
            {
                if (column.ToUpper() == "CIDADE")
                {
                    for (int i = 0; i < dataBySelectedHeaders.Count; i++)
                    {
                        dictonaryByService = dataBySelectedHeaders.Where(data => data.ContainsKey(column)).Where(data => data.Values.Contains(service)).ToList();
                    }
                }
            });

            dictonaryByService.ForEach(dict =>
            {
                cidades.Add(dict["CIDADE"]);
            });

            cidades = cidades.Distinct().ToList();

            ViewBag.Cities = cidades;
            ViewBag.Service = service;

            WriteFiles.WriteStringInFolder(null, service, "service", _appEnvironment.WebRootPath);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Equips(string cidade)
        {
            var listEquipsByCidade = await new ConnectToEquipeApi().GetEquipByCity(cidade);

            WriteFiles.WriteStringInFolder(null, cidade, "city", _appEnvironment.WebRootPath);

            ViewBag.Cidade = cidade;

            return View(listEquipsByCidade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GeradorRota(List<string> equipesSelecionadas)
        {
            List<Equipe> listEquipesByEquipeNome = new();

            listEquipesByEquipeNome = await new ConnectToEquipeApi().GetEquipsByEquipsName(equipesSelecionadas);

            var service = ReadFiles.ReadFileStringInFolder("service", _appEnvironment.WebRootPath);
            var cidade = ReadFiles.ReadFileStringInFolder("city", _appEnvironment.WebRootPath);
            var columns = ReadFiles.ReadFileInFolder("headers", _appEnvironment.WebRootPath);
            var dataBySelectedHeaders = ReadFiles.ReadExcelFile(columns, _appEnvironment.WebRootPath);

            List<string> cidades = new();
            List<IDictionary<string, string>> dictonaryByServiceAndCidade = new();

            columns.ForEach(column =>
            {
                if (column.ToUpper() == "SERVIÇO")
                {
                    for (int i = 0; i < dataBySelectedHeaders.Count; i++)
                    {
                        dictonaryByServiceAndCidade = dataBySelectedHeaders
                            .Where(data => data.ContainsKey(column))
                            .Where(data => data.Values.Contains(service))
                            .Where(data => data.Values.Contains(cidade))
                            .ToList();
                    }
                }
            });

            List<string> otherColumns = new();

            columns.ForEach(column =>
            {
                if (column != "OS" && column != "CIDADE" && column != "BASE" && column != "SERVIÇO" && column != "ENDEREÇO"
                    && column != "NUMERO" && column != "COMPLEMENTO" && column != "CEP" && column != "BAIRRO")
                {
                    otherColumns.Add(column);
                }
            });

            /* Begin Header DOC */

            Document document = new();

            Section section = document.AddSection();

            Paragraph emptyParagraph = section.AddParagraph();
            emptyParagraph.AppendText(" ");
            emptyParagraph = section.AddParagraph();
            emptyParagraph.AppendText(" ");

            Paragraph paragraphTitle = section.AddParagraph();
            TextRange trTitle = paragraphTitle.AppendText($"ROTA DE TRABALHO - {DateTime.Now.Date.ToString("dd/MM/yyyy")}");
            trTitle.CharacterFormat.FontSize = 23;
            trTitle.CharacterFormat.Bold = true;
            trTitle.CharacterFormat.FontName = "Arial";
            paragraphTitle.Format.HorizontalAlignment = HorizontalAlignment.Center;
            paragraphTitle.Format.LineSpacing = 15;

            Paragraph paragraphDate = section.AddParagraph();
            TextRange trDate = paragraphDate.AppendText($"{service}");
            trDate.CharacterFormat.FontSize = 17;
            trDate.CharacterFormat.Bold = true;
            trDate.CharacterFormat.FontName = "Arial";
            paragraphDate.Format.HorizontalAlignment = HorizontalAlignment.Center;

            emptyParagraph = section.AddParagraph();
            emptyParagraph.AppendText(" ");
            emptyParagraph = section.AddParagraph();
            emptyParagraph.AppendText(" ");

            /* End Header DOC */

            /* Begin Equip */

            if (listEquipesByEquipeNome.Count < 2)
            {
                Paragraph paragraphEquipe = section.AddParagraph();
                TextRange trEquipe = paragraphEquipe.AppendText($"Nome da Equipe: {listEquipesByEquipeNome[0].Nome}");
                trEquipe.CharacterFormat.FontSize = 14;
                trEquipe.CharacterFormat.Bold = true;
                trEquipe.CharacterFormat.FontName = "Arial";

                emptyParagraph = section.AddParagraph();
                emptyParagraph.AppendText(" ");

                dictonaryByServiceAndCidade.ForEach(dictionary =>
                {
                    Paragraph paragraph1 = section.AddParagraph();
                    TextRange tr1 = paragraph1.AppendText($"Contrato: {(dictionary.ContainsKey("CONTRATO") ? dictionary["CONTRATO"] : "                     ")} - Assinante: {(dictionary.ContainsKey("ASSINANTE") ? dictionary["ASSINANTE"] : "                            ")} - Período:     :     /     :     .");
                    tr1.CharacterFormat.FontSize = 12;
                    tr1.CharacterFormat.Bold = true;
                    tr1.CharacterFormat.FontName = "Arial";
                    tr1.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;
                    paragraph1.Format.LineSpacing = 15;

                    Paragraph paragraph2 = section.AddParagraph();
                    TextRange tr2 = paragraph2.AppendText($"Endereço: {dictionary["ENDEREÇO"]}, {dictionary["NUMERO"]}, {dictionary["BAIRRO"]}, {dictionary["CIDADE"]}, CEP: {dictionary["CEP"]}  - {(dictionary.ContainsKey("TELEFONE 1") ? dictionary["TELEFONE 1"] : "")}");
                    tr2.CharacterFormat.FontName = "Arial";
                    paragraph2.Format.LineSpacing = 15;

                    Paragraph paragraph3 = section.AddParagraph();
                    TextRange tr3 = paragraph3.AppendText($"O.S.:{dictionary["OS"]} - ");
                    tr3.CharacterFormat.FontName = "Arial";

                    tr3 = paragraph3.AppendText($"Tipo OS: {(dictionary.ContainsKey("TIPO OS") ? dictionary["TIPO OS"] : "_______________")}");
                    tr3.CharacterFormat.TextColor = Color.White;
                    tr3.CharacterFormat.TextBackgroundColor = Color.Red;
                    tr3.CharacterFormat.FontName = "Arial";
                    paragraph3.Format.LineSpacing = 15;

                    if (otherColumns.Count > 0)
                    {
                        for (int col = 0; col < otherColumns.Count; col++)
                        {
                            Paragraph paragraph4 = section.AddParagraph();
                            TextRange tr4 = paragraph4.AppendText($"{otherColumns[col]}: {dictionary[otherColumns[col]]}");
                            tr4.CharacterFormat.FontName = "Arial";
                            paragraph4.Format.LineSpacing = 15;
                        }
                    }

                    emptyParagraph = section.AddParagraph();
                    emptyParagraph.AppendText(" ");
                });
            }
            else if (listEquipesByEquipeNome.Count == dictonaryByServiceAndCidade.Count)
            {
                var i = 0;

                dictonaryByServiceAndCidade.ForEach(dictionary =>
                {
                    Paragraph paragraphEquipe = section.AddParagraph();
                    TextRange trEquipe = paragraphEquipe.AppendText($"Nome da Equipe: {listEquipesByEquipeNome[i].Nome}");
                    trEquipe.CharacterFormat.FontSize = 14;
                    trEquipe.CharacterFormat.Bold = true;
                    trEquipe.CharacterFormat.FontName = "Arial";

                    emptyParagraph = section.AddParagraph();
                    emptyParagraph.AppendText(" ");

                    Paragraph paragraph1 = section.AddParagraph();
                    TextRange tr1 = paragraph1.AppendText($"Contrato: {(dictionary.ContainsKey("CONTRATO") ? dictionary["CONTRATO"] : "                     ")} - Assinante: {(dictionary.ContainsKey("ASSINANTE") ? dictionary["ASSINANTE"] : "                            ")} - Período:     :     /     :     .");
                    tr1.CharacterFormat.FontSize = 12;
                    tr1.CharacterFormat.Bold = true;
                    tr1.CharacterFormat.FontName = "Arial";
                    tr1.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;
                    paragraph1.Format.LineSpacing = 15;

                    Paragraph paragraph2 = section.AddParagraph();
                    TextRange tr2 = paragraph2.AppendText($"Endereço: {dictionary["ENDEREÇO"]}, {dictionary["NUMERO"]}, {dictionary["BAIRRO"]}, {dictionary["CIDADE"]}, CEP: {dictionary["CEP"]}  - {(dictionary.ContainsKey("TELEFONE 1") ? dictionary["TELEFONE 1"] : "")}");
                    tr2.CharacterFormat.FontName = "Arial";
                    paragraph2.Format.LineSpacing = 15;

                    Paragraph paragraph3 = section.AddParagraph();
                    TextRange tr3 = paragraph3.AppendText($"O.S.:{dictionary["OS"]} - ");
                    tr3.CharacterFormat.FontName = "Arial";

                    tr3 = paragraph3.AppendText($"Tipo OS: {(dictionary.ContainsKey("TIPO OS") ? dictionary["TIPO OS"] : "_______________")}");
                    tr3.CharacterFormat.TextColor = Color.White;
                    tr3.CharacterFormat.TextBackgroundColor = Color.Red;
                    tr3.CharacterFormat.FontName = "Arial";
                    paragraph3.Format.LineSpacing = 15;

                    if (otherColumns.Count > 0)
                    {
                        for (int col = 0; col < otherColumns.Count; col++)
                        {
                            Paragraph paragraph4 = section.AddParagraph();
                            TextRange tr4 = paragraph4.AppendText($"{otherColumns[col]}: {dictionary[otherColumns[col]]}");
                            tr4.CharacterFormat.FontName = "Arial";
                            paragraph4.Format.LineSpacing = 15;
                        }
                    }

                    emptyParagraph = section.AddParagraph();
                    emptyParagraph.AppendText(" ");

                    i++;
                });
            }
            else if (listEquipesByEquipeNome.Count > 1)
            {
                int serviceSplit = dictonaryByServiceAndCidade.Count / listEquipesByEquipeNome.Count;
                decimal restSplit = dictonaryByServiceAndCidade.Count % listEquipesByEquipeNome.Count;
                int listEquip = 0;

                for (int k = 0; k < dictonaryByServiceAndCidade.Count; k++)
                {
                    Paragraph paragraphEquipe = section.AddParagraph();
                    TextRange trEquipe = paragraphEquipe.AppendText($"Nome da Equipe: {listEquipesByEquipeNome[listEquip].Nome}");
                    trEquipe.CharacterFormat.FontSize = 14;
                    trEquipe.CharacterFormat.Bold = true;
                    trEquipe.CharacterFormat.FontName = "Arial";

                    emptyParagraph = section.AddParagraph();
                    emptyParagraph.AppendText(" ");

                    listEquip++;

                    for (int i = 0; i < serviceSplit; i++)
                    {
                        var dictionary = dictonaryByServiceAndCidade[k];

                        Paragraph paragraph1 = section.AddParagraph();
                        TextRange tr1 = paragraph1.AppendText($"Contrato: {(dictionary.ContainsKey("CONTRATO") ? dictionary["CONTRATO"] : "                     ")} - Assinante: {(dictionary.ContainsKey("ASSINANTE") ? dictionary["ASSINANTE"] : "                            ")} - Período:     :     /     :     .");
                        tr1.CharacterFormat.FontSize = 12;
                        tr1.CharacterFormat.Bold = true;
                        tr1.CharacterFormat.FontName = "Arial";
                        tr1.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;
                        paragraph1.Format.LineSpacing = 15;

                        Paragraph paragraph2 = section.AddParagraph();
                        TextRange tr2 = paragraph2.AppendText($"Endereço: {dictionary["ENDEREÇO"]}, {dictionary["NUMERO"]}, {dictionary["BAIRRO"]}, {dictionary["CIDADE"]}, CEP: {dictionary["CEP"]}  - {(dictionary.ContainsKey("TELEFONE 1") ? dictionary["TELEFONE 1"] : "")}");
                        tr2.CharacterFormat.FontName = "Arial";
                        paragraph2.Format.LineSpacing = 15;

                        Paragraph paragraph3 = section.AddParagraph();
                        TextRange tr3 = paragraph3.AppendText($"O.S.:{dictionary["OS"]} - ");
                        tr3.CharacterFormat.FontName = "Arial";

                        tr3 = paragraph3.AppendText($"Tipo OS: {(dictionary.ContainsKey("TIPO OS") ? dictionary["TIPO OS"] : "_______________")}");
                        tr3.CharacterFormat.TextColor = Color.White;
                        tr3.CharacterFormat.TextBackgroundColor = Color.Red;
                        tr3.CharacterFormat.FontName = "Arial";
                        paragraph3.Format.LineSpacing = 15;

                        if (otherColumns.Count > 0)
                        {
                            for (int col = 0; col < otherColumns.Count; col++)
                            {
                                Paragraph paragraph4 = section.AddParagraph();
                                TextRange tr4 = paragraph4.AppendText($"{otherColumns[col]}: {dictionary[otherColumns[col]]}");
                                tr4.CharacterFormat.FontName = "Arial";
                                paragraph4.Format.LineSpacing = 15;
                            }
                        }

                        emptyParagraph = section.AddParagraph();
                        emptyParagraph.AppendText(" ");

                        k++;
                    }

                    k--;

                    if (k == dictonaryByServiceAndCidade.Count - 2)
                    {
                        k++;

                        if (restSplit > 0)
                        {
                            var dictionary = dictonaryByServiceAndCidade[dictonaryByServiceAndCidade.Count - 1];

                            Paragraph paragraph1 = section.AddParagraph();
                            TextRange tr1 = paragraph1.AppendText($"Contrato: {(dictionary.ContainsKey("CONTRATO") ? dictionary["CONTRATO"] : "                     ")} - Assinante: {(dictionary.ContainsKey("ASSINANTE") ? dictionary["ASSINANTE"] : "                            ")} - Período:     :     /     :     .");
                            tr1.CharacterFormat.FontSize = 12;
                            tr1.CharacterFormat.Bold = true;
                            tr1.CharacterFormat.FontName = "Arial";
                            tr1.CharacterFormat.UnderlineStyle = UnderlineStyle.Single;
                            paragraph1.Format.LineSpacing = 15;

                            Paragraph paragraph2 = section.AddParagraph();
                            TextRange tr2 = paragraph2.AppendText($"Endereço: {dictionary["ENDEREÇO"]}, {dictionary["NUMERO"]}, {dictionary["BAIRRO"]}, {dictionary["CIDADE"]}, CEP: {dictionary["CEP"]}  - {(dictionary.ContainsKey("TELEFONE 1") ? dictionary["TELEFONE 1"] : "")}");
                            tr2.CharacterFormat.FontName = "Arial";
                            paragraph2.Format.LineSpacing = 15;

                            Paragraph paragraph3 = section.AddParagraph();
                            TextRange tr3 = paragraph3.AppendText($"O.S.:{dictionary["OS"]} - ");
                            tr3.CharacterFormat.FontName = "Arial";

                            tr3 = paragraph3.AppendText($"Tipo OS: {(dictionary.ContainsKey("TIPO OS") ? dictionary["TIPO OS"] : "_______________")}");
                            tr3.CharacterFormat.TextColor = Color.White;
                            tr3.CharacterFormat.TextBackgroundColor = Color.Red;
                            tr3.CharacterFormat.FontName = "Arial";
                            paragraph3.Format.LineSpacing = 15;

                            if (otherColumns.Count > 0)
                            {
                                for (int col = 0; col < otherColumns.Count; col++)
                                {
                                    Paragraph paragraph4 = section.AddParagraph();
                                    TextRange tr4 = paragraph4.AppendText($"{otherColumns[col]}: {dictionary[otherColumns[col]]}");
                                    tr4.CharacterFormat.FontName = "Arial";
                                    paragraph4.Format.LineSpacing = 15;
                                }
                            }

                            emptyParagraph = section.AddParagraph();
                            emptyParagraph.AppendText(" ");
                        }
                    }
                }
            }

            service = service.Replace("Ç", "C").Replace("ç", "c").Replace("Ã", "A").Replace("ã", "a").Replace("É", "e").Replace("é", "e").Replace(" ", "");
            cidade = cidade.Replace("Ç", "C").Replace("ç", "c").Replace("Ã", "A").Replace("ã", "a").Replace("É", "e").Replace("é", "e").Replace(" ", "");

            string nameFile = $"Route{service}{cidade}{DateTime.Now.Date.ToString("ddMMyyyy")}.docx";

            document.SaveToFile(_appEnvironment.WebRootPath + "\\File\\" + nameFile, FileFormat.Docx);

            /* FIM DOC*/

            Arquivo historyGenerateFile = new();
            historyGenerateFile.Data = DateTime.Now.Date.ToString("dd/MM/yyyy");
            historyGenerateFile.Servico = service;
            historyGenerateFile.Cidade = cidade;
            historyGenerateFile.Colunas = columns;
            historyGenerateFile.Equipes = equipesSelecionadas;
            historyGenerateFile.NomeArquivo = nameFile;
            historyGenerateFile.CaminhoArquivo = _appEnvironment.WebRootPath + "\\File\\" + nameFile;

            _gRotasService.Create(historyGenerateFile);

            RemoveFiles.RemoveFromFolder("headers", ".txt", _appEnvironment.WebRootPath);
           // RemoveFiles.RemoveFromFolder("service", ".txt", _appEnvironment.WebRootPath);
          //  RemoveFiles.RemoveFromFolder("city", ".txt", _appEnvironment.WebRootPath);

            return View();
        }

      /*  public ActionResult DownloadFiles()
        {
            string usuario = "Anonymous";
            bool authenticate = false;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                usuario = HttpContext.User.Identity.Name;
                authenticate = true;

                if (HttpContext.User.IsInRole("adm"))
                    ViewBag.Role = "adm";
                else
                    ViewBag.Role = "user";
            }
            else
            {
                usuario = "Não Logado";
                authenticate = false;
                ViewBag.Role = "";
            }

            ViewBag.User = usuario;
            ViewBag.Authenticate = authenticate;

            List<Arquivo> hgFile = _gRotasService.Get();

            return View(hgFile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DownloadFiles(Arquivo hgFile)
        {


            return View();
        }*/

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public FileResult DownloadFile()
        {
            var service = ReadFiles.ReadFileStringInFolder("service", _appEnvironment.WebRootPath);
            var city = ReadFiles.ReadFileStringInFolder("city", _appEnvironment.WebRootPath);

            service = service.Replace("Ç", "C").Replace("ç", "c").Replace("Ã", "A").Replace("ã", "a").Replace("É", "e").Replace("é", "e").Replace(" ", "");
            city = city.Replace("Ç", "C").Replace("ç", "c").Replace("Ã", "A").Replace("ã", "a").Replace("É", "e").Replace("é", "e").Replace(" ", "");

            string nameFile = $"Route{service}{city}{DateTime.Now.Date.ToString("ddMMyyyy")}.docx";

            string folder = "\\File\\";
            string pathFinal = _appEnvironment.WebRootPath + folder + nameFile;

            byte[] bytes = System.IO.File.ReadAllBytes(pathFinal);

            string contentType = "application/octet-stream";

            RemoveFiles.RemoveFromFolder("service", ".txt", _appEnvironment.WebRootPath);
            RemoveFiles.RemoveFromFolder("city", ".txt", _appEnvironment.WebRootPath);

            return File(bytes, contentType, nameFile);
        }
        

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var file = _gRotasService.Get(id);

            if (file == null)
            {
                return NotFound();
            }

            return View(file);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string id, Arquivo hgFile)
        {
            _gRotasService.Delete(id);

            /*if (hgFileRemove.Error != "ok")
                return BadRequest("Pessoa - Houve um erro na exclusão do usuário. Favor tentar novamente");*/

            return RedirectToAction(nameof(Index));
        }
    }
}
