using UnityEngine;

namespace _Scripts.Player
{
    [System.Serializable]
    public sealed class PlayerEnergyModule
    {
        private readonly float _maxEnergy;
        private readonly float _energyRegenRate;

        private float _energy;

        public float Energy => _energy;
        public float MaxEnergy => _maxEnergy;

        public PlayerEnergyModule(float maxEnergy, float energyRegenRate, float startingEnergy)
        {
            _maxEnergy = maxEnergy;
            _energyRegenRate = energyRegenRate;
            _energy = Mathf.Clamp(startingEnergy, 0f, _maxEnergy);
        }

        public void SetEnergy(float value)
        {
            _energy = Mathf.Clamp(value, 0f, _maxEnergy);
        }

        public void ResetEnergy()
        {
            _energy = _maxEnergy;
        }

        public bool SpendEnergy(float amount)
        {
            if (_energy <= 0.17f)
                return false;

            float consumed = Mathf.Min(amount, _energy);
            _energy -= consumed;
            return true;
        }

        public void RegenEnergy(float dt)
        {
            if (_energy < _maxEnergy)
                _energy = Mathf.Min(_maxEnergy, _energy + _energyRegenRate * dt);
        }

        public void ConsumeForced(float amount)
        {
            if (amount <= 0f)
                return;

            _energy = Mathf.Max(0f, _energy - amount);
        }

        public byte QuantizeEnergy()
        {
            return (byte)Mathf.Clamp(Mathf.RoundToInt(_energy / _maxEnergy * 255f), 0, 255);
        }

        public float DequantizeEnergy(byte b)
        {
            return b / 255f * _maxEnergy;
        }

        public void ApplyQuantizedEnergy(byte b)
        {
            _energy = DequantizeEnergy(b);
        }
    }
}