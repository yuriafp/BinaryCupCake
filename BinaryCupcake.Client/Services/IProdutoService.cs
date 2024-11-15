﻿using BinaryCupcake.SharedLibrary.Models;
using BinaryCupcake.SharedLibrary.Responses;

namespace BinaryCupcake.Client.Services
{
    public interface IProdutoService
    {
        Task<ServiceResponse> AddProduto(Produto produto);
        Task<List <Produto>> ListaTodosProdutos(bool produtoDestacado);
    }
}
