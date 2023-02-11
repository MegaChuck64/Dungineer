using System;
using System.Collections.Generic;
using System.Text;

namespace Engine;

public interface IPrefab<T>
{
    public T Instantiate(BaseGame game);
}