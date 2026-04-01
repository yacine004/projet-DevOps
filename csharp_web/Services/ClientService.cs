using csharp_web.Models;
using csharp_web.Repositories;

namespace csharp_web.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        public async Task<Client?> AuthenticateAsync(string nom, string telephone)
        {
            return await _clientRepository.GetByNomAndTelephoneAsync(nom, telephone);
        }

        public async Task RegisterAsync(string nom, string prenom, string telephone)
        {
            var client = new Client
            {
                Nom = nom,
                Prenom = prenom,
                Telephone = telephone
            };
            await _clientRepository.AddAsync(client);
        }

        public async Task<bool> IsRegisteredAsync(string nom, string telephone)
        {
            return await _clientRepository.ExistsAsync(nom, telephone);
        }
    }
}