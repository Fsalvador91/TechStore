using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TechStore.Catalogo.Application.Services;
using TechStore.Core.Communication.Mediator;
using TechStore.Core.Messages.Notifications;
using TechStore.Vendas.Application.Commands;
using TechStore.Vendas.Application.Queries;
using TechStore.Vendas.Application.Queries.ViewModels;

namespace TechStore.WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarrinhoController : ControllerBase
    {
        private readonly IProdutoAppService _produtoAppService;
        private readonly IPedidosQueries _pedidosQueries;
        private readonly IMediatorHandler _mediatorHandler;

        public CarrinhoController(INotificationHandler<DomainNotification> notifications,
                                  IProdutoAppService produtoAppService,
                                  IMediatorHandler mediatorHandler,
                                  IPedidosQueries pedidosQueries) : base(notifications, mediatorHandler)
        {
            _produtoAppService = produtoAppService;
            _mediatorHandler = mediatorHandler;
            _pedidosQueries = pedidosQueries;
        }

        [HttpGet]
        [Route("meu-carrinho")]
        public async Task<IActionResult> Index()
        {
            return View(await _pedidosQueries.ObterCarrinhoCliente(ClienteId));
        }

        [HttpPost]
        [Route("pedidos")]
        public async Task<IActionResult> IniciarPedido(CarrinhoViewModel carrinhoViewModel)
        {
            var carrinho = await _pedidosQueries.ObterCarrinhoCliente(ClienteId);

            var command = new IniciarPedidoCommand(carrinho.PedidoId, ClienteId, carrinho.ValorTotal, carrinhoViewModel.Pagamento.NomeCartao,
                carrinhoViewModel.Pagamento.NumeroCartao, carrinhoViewModel.Pagamento.ExpiracaoCartao, carrinhoViewModel.Pagamento.CvvCartao);

            await _mediatorHandler.EnviarComando(command);

            if (OperacaoValida())
            {
                return Ok();
            }

            return Content("Erro");
        }
    }
}
