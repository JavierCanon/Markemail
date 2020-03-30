namespace Eron.MailClient.SharedKernel.Modules.Helpers.Mapper
{
    public static class ObjectMapper
    {
        public static TTarget MapTo<TTarget>(this object data)
        {
            return AutoMapper.Mapper.Map<TTarget>(data);
            //return TinyMapper.Map<TTarget>(data);
        }
    }
}
