// Copyright eeGeo Ltd (2012-2014), All Rights Reserved

using Wrld.MapInput.Mouse;
using Wrld.MapInput.Touch;

namespace Wrld.MapInput
{
    public interface IUnityInputProcessor
    {
        void HandleInput(MouseInputEvent inputEvent);
        void HandleInput(TouchInputEvent inputEvent);

        void Update(float deltaSeconds);

    };
}
