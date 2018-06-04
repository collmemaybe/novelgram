namespace Src
{
    using Amazon.S3.Transfer;

    using Autofac;

    using Src.Models.Photo;
    using Src.Models.User;

    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TransferUtility>().As<ITransferUtility>().InstancePerLifetimeScope();
            builder.RegisterType<NovelGramPhotoFactory>().As<INovelGramPhotoFactory>().InstancePerLifetimeScope();
            builder.RegisterType<NovelGramUserClient>().As<INovelGramUserClient>().InstancePerLifetimeScope();
            builder.RegisterType<CurrentUserManager>().As<ICurrentUserManager>().InstancePerLifetimeScope();
            builder.RegisterType<NovelGramUserClient>().As<INovelGramUserClient>().InstancePerLifetimeScope();
            builder.RegisterType<PhotoService>().As<IPhotoService>().InstancePerLifetimeScope();
        }
    }
}
