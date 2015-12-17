using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediatrOpGen
{
    using MediatR;

    

    public class InputObject : IRequest<OutputObject>
    {
        
    }

    public class Handler : IRequestHandler<InputObject,OutputObject>
    {
      
        public OutputObject Handle(InputObject message)
        {
            Console.WriteLine("Handled");
            return new OutputObject();
        }
    }

    public class ClosedPreRequestHandler : IPreRequestHandler<InputObject>
    {
        public void Handle(InputObject request)
        {
            Console.WriteLine("Hit Close Pre Request Handler");
        }
    }

    public class OpenPreRequestHandler<TObject> : IPreRequestHandler<TObject>
    {
     

        public void Handle(TObject request)
        {
            Console.WriteLine("Hit open Pre Request Handler");
        }
    }

    public class ClosedPostRequestHandler : IPostRequestHandler<InputObject,OutputObject>
    {
      

        public void Handle(InputObject request, OutputObject response)
        {
            Console.WriteLine("Hit Close Post Request Handler");
        }
    }

    public class OpenPostRequestHandler<TObject,TResult> : IPostRequestHandler<TObject, TResult>
    {


        public void Handle(TObject request,TResult Result)
        {
            Console.WriteLine("Hit open Post Request Handler");
        }
    }
}
