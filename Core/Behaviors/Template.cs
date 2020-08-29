// using Chains;

// namespace Core.Behaviors
// {
//     public class Template : Behavior
//     {
//         public class Config : BehaviorConfig
//         {

//         }

//         public class Params : ActivationParams
//         {

//         }

//         Chain chain_myChain;

//         public Template(Entity entity, Config conf)
//         {
//             var config = (Config)conf;
//             chain_myChain = entity.m_chains["myChain:check"];
//         }

//         public override bool Activate(Entity entity, Action action, ActivationParams pars)
//         {
//             var p = (Params)pars;
//             return true;
//         }

//         public static BehaviorFactory s_factory = new BehaviorFactory(
//             typeof(Template), new ChainDefinition<CommonEvent>[] {
//                 new ChainDefinition<CommonEvent>
//                 {
//                     name = "",
//                     handlers = new WeightedEventHandler<CommonEvent>[]
//                     {
//                         // new WeightedEventHandler<CommonEvent>
//                         // {
//                         //     handlerFunction = func
//                         // }
//                     }
//                 }
//             });

//     }
// }