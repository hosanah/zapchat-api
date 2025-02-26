using AutoMapper;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zapchat.Domain.DTOs;
using Zapchat.Domain.Entities;
using Zapchat.Domain.Interfaces;
using Zapchat.Domain.Interfaces.Messages;

namespace Zapchat.Service.Services
{
    public class ParametroSistemaService : BaseService, IParametroSistemaService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IValidator<UsuarioDto> _validator;
        private readonly IMapper _mapper;
        private readonly IParametroSistemaRepository _parametroSistemaRepository;
        private readonly IGrupoWhatsAppService _grupoWhatsAppService;

        public ParametroSistemaService(IUsuarioRepository usuarioRepository, 
            IValidator<UsuarioDto> validator, 
            IMapper mapper, 
            INotificator notificator,
            IParametroSistemaRepository parametroSistemaRepository,
            IGrupoWhatsAppService grupoWhatsAppService) : base(notificator)
        {
            _usuarioRepository = usuarioRepository;
            _validator = validator;
            _mapper = mapper;
            _parametroSistemaRepository = parametroSistemaRepository;
            _grupoWhatsAppService = grupoWhatsAppService;
        }

        public async Task<ParamGrupoWhatsApp> BuscarParammetroPorGrupoIdentificador(string grupoIdentificador)
        {
            var grupo = await _grupoWhatsAppService.GetByIdentificadorAsync(grupoIdentificador);
            if (grupo == null)
            {
                Notify("Não foi encontrado nenhum grupo com o identificador informado");
                return null;
            }
            return await _parametroSistemaRepository.GetByGrupoIdAsync(grupo.Id);
        }
    }
}
