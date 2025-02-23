using Zapchat.Domain.Interfaces;
using AutoMapper;
using Zapchat.Domain.DTOs;
using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Zapchat.Domain.Entities;
using Zapchat.Domain.Notifications;
using Microsoft.Extensions.Configuration;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Service.Services
{
    public class GrupoWhatsAppService : BaseService, IGrupoWhatsAppService
    {
        private readonly IGrupoWhatsAppRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IAdmsGrupoRepository _admsRepository;
        private readonly IParametroSistemaRepository _paramRepository;
        private readonly IUtilsService _utilsService;

        public GrupoWhatsAppService(IGrupoWhatsAppRepository repository, IConfiguration configuration, INotificator notificator, IMapper mapper, IAdmsGrupoRepository admsRepository, IParametroSistemaRepository paramRepository, IUtilsService utilsService) : base(notificator)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
            _admsRepository = admsRepository;
            _paramRepository = paramRepository;
            _utilsService = utilsService;
        }

        public async Task<GrupoWhatsAppDto> AddAsync(GrupoWhatsAppDto grupoDto)
        {
            try
            {
                if (grupoDto == null)
                {
                    Notify("Os dados do grupo são inválidos.");
                    return null;
                }

                var novoGrupo = _mapper.Map<GrupoWhatsApp>(grupoDto);

                await _repository.AddAsync(novoGrupo);

                grupoDto.Id = novoGrupo.Id; 

                return grupoDto;
            }
            catch (Exception ex)
            {
                Notify($"Erro ao inserir o grupo: {ex.Message}");
                return null;
            }
        }

        public async Task<AutoConfigurarGrupoDto> AutoConfigurarGrupo(AutoConfigurarGrupoDto configDto)
        {
            try
            {
                var validacoes = new List<string>();
                var grupos = await _repository.GetAllAsync();
                if (grupos.Any(e => e.Identificador.Equals(configDto.GrupoIdentificador)))
                    validacoes.Add("Já existe um grupo cadastrado com o mesmo identificados!");

                var parametros = await _paramRepository.GetAllAsync();
                if (parametros.Any(e => e.AppKey.Equals(configDto.ApiKey)))
                    validacoes.Add("Já existe um ApiKey cadastrado com o mesmo identificados!");

                if (parametros.Any(e => e.AppSecret.Equals(configDto.ApiSecrect)))
                    validacoes.Add("Já existe um ApiSecrect cadastrado com o mesmo identificados!");

                if (validacoes.Any())
                {
                    foreach (var validacao in validacoes)
                    {
                        Notify(validacao);
                    }

                    return null;
                }

                // ✅ Criando entidade GrupoWhatsApp
                var novoGrupo = new GrupoWhatsApp(configDto);
                await _repository.AddAsync(novoGrupo);

                // ✅ Criando lista de AdmGrupoWhatsApp
                var adms = configDto.AdmDto;

                if (adms.Any())
                {
                    foreach (var adm in adms)
                    {
                        var admGrupo = new AdmGrupoWhatsApp(adm.NumeroAdm);
                        admGrupo.GrupoId = novoGrupo.Id;
                        await _admsRepository.AddAsync(admGrupo);
                    }
                }

                // ✅ Criando lista de ParamGrupoWhatsApp
                var parametro = new ParamGrupoWhatsApp(configDto.ApiKey, configDto.ApiSecrect);
                parametro.GrupoId = novoGrupo.Id;
                await _paramRepository.AddAsync(parametro);
                configDto.GrupoGid = novoGrupo.Id;
                return configDto;
            }
            catch (Exception ex)
            {
                Notify($"Erro ao configurar o grupo: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<AutoConfigurarGrupoDto>> BuscarTodasConfigurações()
        {
            var grupos = await _repository.GetAllAsync();

            var resultado = new List<AutoConfigurarGrupoDto>();

            foreach (var grupo in grupos)
            {
                var administradores = await _admsRepository.GetByGrupoIdAsync(grupo.Id);

                var parametros = await _paramRepository.GetByGrupoIdAsync(grupo.Id);

                var dto = new AutoConfigurarGrupoDto
                {
                    GrupoGid = grupo.Id,
                    GrupoNome = grupo.Nome,
                    GrupoIdentificador = grupo.Identificador,
                    ApiKey = parametros.AppKey,
                    ApiSecrect = parametros.AppSecret,
                    AdmDto = administradores.Select(adm => new AdmDto
                    {
                        NumeroAdm = adm.NumeroAdm
                    }).ToList()
                };

                resultado.Add(dto);
            }

            return resultado;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                await _paramRepository.DeleteByGrupoIdAsync(id);
                await _admsRepository.DeleteByGrupoIdAsync(id);
                await _repository.DeleteAsync(id);
                return true;
            }   
            catch (Exception)
            {

                Notify("Já existe um grupo cadast   rado com o mesmo identificados!");
                return false;
            }
        }

        public Task<IEnumerable<GrupoWhatsAppDto>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<GrupoWhatsAppDto?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<GrupoWhatsAppDto?> UpdateAsync(Guid id, GrupoWhatsAppDto usuarioDto)
        {
            throw new NotImplementedException();
        }
    }
}
