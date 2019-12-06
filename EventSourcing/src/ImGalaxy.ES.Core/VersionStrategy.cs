namespace ImGalaxy.ES.Core
{
    public static class VersionStrategy
    {
        public static bool IsAppliedByIVersion(IAggregateRoot root) =>
            root.GetType().IsAssignableFrom(typeof(IVersion));

        public static long VersionOfRoot(IAggregateRoot root) =>
            (root as IVersion).Version;

    }
}
