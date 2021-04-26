using Hopper.Shared.Attributes;
using Hopper.Core.Components.Basic;

namespace Hopper.Core
{
    public class BindingEntityModifier : IEntityModifier
    {
        // This may contain any data that the handlers require
    }

    [InstanceExport]
    public partial class Bind
    {
        [InstanceExport] public static Bind Example = new Bind(1);

        public EntityModifierIndex<BindingEntityModifier> Index;

        [Export(Chain = "Ticking.Do", Priority = PriorityRank.High, Dynamic = true)] 
        public void Remove(Entity actor) => Index.TryRemoveFrom(actor);


        [Export(Chain = "Ticking.Do", Priority = PriorityRank.High, Dynamic = true)] 
        public void DoSomeStuffWithComponent(Entity actor)
        {
            if (actor.TryGetComponent(Index.ComponentIndex, out var component))
            {
                // component.DoSomeStuff();
            }
        }

        public Bind(int someParameter)
        {
            Index = new EntityModifierIndex<BindingEntityModifier>(InstantiateAndBind, Unbind);
            // this.someField = someParameter;
        }

        public BindingEntityModifier InstantiateAndBind(Entity actor) 
        {
            RemoveHandlerWrapper.HookTo(actor);
            DoSomeStuffWithComponentHandlerWrapper.HookTo(actor);
            return new BindingEntityModifier(/* someParameter passed along */);
        }

        public void Unbind(BindingEntityModifier component, Entity actor)
        {
            RemoveHandlerWrapper.UnhookFrom(actor);
            DoSomeStuffWithComponentHandlerWrapper.UnhookFrom(actor);
        }
    }
}