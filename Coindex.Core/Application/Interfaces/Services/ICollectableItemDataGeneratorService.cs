using Coindex.Core.Domain.Entities;
using Coindex.Core.Domain.Enums;

namespace Coindex.Core.Application.Interfaces.Services;

public interface ICollectableItemDataGeneratorService
{
    string GenerateRandomName(bool isCoin, int? number = null);
    string GenerateRandomDescription(bool isCoin, string country, int year);
    int GenerateRandomYear(bool isCoin);
    string GenerateRandomCountry();
    decimal GenerateRandomFaceValue(bool isCoin);
    ItemCondition GenerateRandomCondition();
    string GenerateRandomMint();
    string GenerateRandomMaterial();
    decimal GenerateRandomWeight();
    decimal GenerateRandomDiameter();
    string GenerateRandomSeries(int year);
    string GenerateRandomSerialNumber();
    string GenerateRandomSignatureType();
    string GenerateRandomBillType();
    decimal GenerateRandomWidth();
    decimal GenerateRandomHeight();
    List<Tag> GetRandomTags(List<Tag> availableTags);
}
