using BankingSystem.API.DTOs;
using BankingSystem.API.Entities;
using Microsoft.AspNetCore.JsonPatch;

namespace BankingSystem.API.Data.Repository.IRepository
{
    public interface IKycRepository
    {
        Task<IEnumerable<KycDocument>> GetKycDocumentAsync();
        Task<KycDocument?> GetKYCIdAsync(Guid KYCId);
        Task<KycDocument> GetKycByUserIdAsync(Guid userId);
        Task<KycDocument> AddKycDocumentAsync(KycDocument kycDocument);
        Task<KycDocument> UpdateKycDocumentAsync(Guid KYCId, KycDocument updatedKycDocument);
        public Task<KycDocument> UpdateKycDocumentAsync(Guid KYCId, JsonPatchDocument<KycDocumentDTO> kycDetails);
    }
}
