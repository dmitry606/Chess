﻿using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess.Tests.Models
{
    public class WritablePropertiesOnlyResolver : DefaultContractResolver
	{
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
			return props.Where(p => p.Writable).ToList();
		}
	}
}
