using AutoMapper;
using Business;
using Business.Interfaces;
using DataAccess.Entities;
using DataTransferObject;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using StaticClass;
using System;
using Website.Models.Formulario;
using Website.Pages.Shared.Components;

namespace Website.Pages.Tramites.Formulario.Form6
{
    public partial class BalanceGeneralEvaluar : ComponentBase
    {
        [Parameter] public int IdTramite { get; set; }
        [Parameter] public int IdGrupoTramite { get; set; }
        [Parameter] public BalanceGeneral_Evaluar_Model Model { get; set; } = null!;
        [Parameter] public bool ReadOnly { get; set; } = true;

        [Inject] private IFilesBL _filesBL { get; set; } = null!;
        [Inject] private ITramitesEvaluacionBL _TramiteEvaluacionBL { get; set; } = null!;
        [Inject] NavigationManager navigationManager { get; set; } = null!;
        private IMapper _mapper { get; set; } = null!;

        private CustomFileUpload? upload;
        private List<FileDTO> lstFiles = new();
        private bool isBusyRefresh = false;
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            //Crear el mapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<BalanceGeneral_Evaluar_Model, TramitesBalanceGeneralDTO>();
            });
            _mapper = config.CreateMapper();
            //--


            if (Model.IdFile.HasValue)
                lstFiles.Add(await _filesBL.GetFileAsync(Model.IdFile.Value));

        }
        
        void OnRenderCapTecnica2(DataGridRenderEventArgs<BalanceGeneral_CapTecItem_Evaluar_Model> args)
        {
            if (args.FirstRender)
            {
                if(args.Grid.Groups.Count <= 0) { 
                    args.Grid.Groups.Add(new GroupDescriptor() { Property = "DescripcionSeccion", Title = "Especialidad" });
                    StateHasChanged();
                }
            }
        }
        private async Task ActualizarDatosDesdeObrasClick()
        {
            isBusyRefresh = true;
            Model.CoeficienteConceptual = await _TramiteEvaluacionBL.GetPromedioCoeficienteConceptualAsync(Model.IdTramite);
            if (Model.IdTipoTramite == Constants.TiposDeTramite.Reli_Licitar)
            {
                var ObraDto = await _TramiteEvaluacionBL.GetDatosObraLicitarAsync(Model.IdTramite);
                Model.MontoObra = ObraDto.MontoObra.GetValueOrDefault();
            }
            isBusyRefresh = false;
        }

        private async Task SaveData()
        {
            var dto = _mapper.Map<TramitesBalanceGeneralDTO>(Model);
            await _TramiteEvaluacionBL.ActualizarCamposBalanceGeneralEvaluadorAsync(dto);
            navigationManager.NavigateTo(navigationManager.Uri, true);
        }
    }
}
