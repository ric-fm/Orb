public struct Trigger
{
	private bool oldValue;

	public bool Check(bool value)
	{
		bool result = value && !oldValue;
		oldValue = value;

		return result;
	}
}
